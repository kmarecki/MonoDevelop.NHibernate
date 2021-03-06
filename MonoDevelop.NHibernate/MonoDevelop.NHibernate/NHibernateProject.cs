// 
// NHibernateProject.cs
//  
// Author:
//       Krzysztof Marecki
// 
// Copyright (c) 2010 KrzysztofMarecki
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Tasks;
using MonoDevelop.Projects;
using Mono.TextTemplating;

namespace MonoDevelop.NHibernate
{
	public class NHibernateProject
	{
		DotNetProject project;
		string langExtension;
		
		SimpleFileTemplate codeFileTemplate;
		SimpleFileTemplate CodeFileTemplate {
			get {
				if (codeFileTemplate == null) {
					codeFileTemplate = new SimpleFileTemplate ();
					codeFileTemplate.ReadTemplate (ClassTemplateFile);
				}
				return codeFileTemplate;
			}
		}
		
		TemplateGenerator generator;
		TemplateGenerator Generator {
			get {
				if (generator == null) {
					generator = new TemplateGenerator ();
					
					var configuration = (DotNetProjectConfiguration) project.DefaultConfiguration;
					generator.Refs.Add (configuration.CompiledOutputName);
					generator.ReferencePaths.Add (configuration.CompiledOutputName.ParentDirectory);
				
					var assembly = Assembly.GetExecutingAssembly ();
					generator.Refs.Add (assembly.FullName);
					generator.ReferencePaths.Add (Path.GetDirectoryName(assembly.CodeBase));
				}
				return generator;
			}
		}
		
		public NHibernateProject (DotNetProject project)
		{
			this.project = project;
			
			var binding = LanguageBindingService.GetBindingPerLanguageName (project.LanguageName) as IDotNetLanguageBinding;
			CodeDomProvider provider = binding.GetCodeDomProvider ();
			if (provider == null)
				throw new UserException ("Code generation not supported for language: " + project.LanguageName);
			langExtension = "." + provider.FileExtension;
		}
		
		public FilePath ClassTemplateFile {
			get { return TemplatesFolder.Combine ("class_template.tt"); }
		}
	
		public FilePath TemplatesFolder { 
			get { return project.BaseDirectory.Combine ("NHibernateTemplates"); }
		}
		
		public string HbmFileExtension {
			get { return "hbm.xml"; }
		}
		
		public bool Exists {
			get { return Directory.Exists (TemplatesFolder); }
		}
		
		public void Create ()
		{
			if (!Directory.Exists (TemplatesFolder)) {
				FileService.CreateDirectory (TemplatesFolder);
				project.AddDirectory (TemplatesFolder);
			}
			if (!File.Exists (ClassTemplateFile)) {
				using (StreamWriter stream = File.CreateText (ClassTemplateFile)) {
				}
				project.AddFile (ClassTemplateFile, BuildAction.None);
			}
		}
		
		public void GenerateCodeFile (ProjectFile pf)
		{
			string classTemplateFile = GetClassTemplateFileName (pf);
			string codeFile = GetCodeFileName (pf);
			
			try {
				CodeFileTemplate.WriteFor (pf, classTemplateFile);
				Generator.ProcessTemplate (classTemplateFile, codeFile);
				ShowTemplateGeneratorErrors (Generator.Errors);
				
				if (File.Exists (codeFile) && !project.IsFileInProject (codeFile)) {
					ProjectFile pfCode = project.AddFile (codeFile, BuildAction.Compile);
					pfCode.DependsOn = pf.Name;
				}
			} finally {
				if (File.Exists (classTemplateFile))
					File.Delete (classTemplateFile);
			}
		}
		
		public void CheckAndGenerateCodeFiles ()
		{
			List<ProjectFile> dirtyfiles = new List<ProjectFile> ();
			
			foreach (ProjectFile pf in project.Files) {
				if (!IsHbmFile (pf))
					continue;
				FileInfo hbmFile = new FileInfo (pf.FilePath);
				if (hbmFile == null)
					continue;
				FileInfo codeFile = new FileInfo (GetCodeFileName (pf));
				if (codeFile == null || 
				    hbmFile.LastWriteTime > codeFile.LastWriteTime) {
					dirtyfiles.Add (pf);
				}
			}
			foreach (ProjectFile pf in dirtyfiles) {
				GenerateCodeFile (pf);
			}
		}
		
		FilePath GetClassTemplateFileName (ProjectFile pf)
		{
			string fileName = string.Format ("{0}.tt", pf.Name);
			string path = Path.Combine (Path.GetTempPath (), fileName);
			return new FilePath (path);
		}
		
		FilePath GetCodeFileName (ProjectFile pf)
		{
			string fileName = pf.FilePath.ToString ().Replace (HbmFileExtension, "hbm" +langExtension);
			return fileName;
		}
		
		public bool IsHbmFile (ProjectFile pf)
		{
			return pf.Name.Contains (HbmFileExtension);
		}
		
		void ShowTemplateGeneratorErrors (CompilerErrorCollection errors)
		{
			if (errors.Count == 0)
				return;
			
			TaskService.Errors.Clear ();
			foreach (CompilerError err in errors) {
					TaskService.Errors.Add (new Task (err.FileName, err.ErrorText, err.Column, err.Line,
					                                  err.IsWarning? TaskSeverity.Warning : TaskSeverity.Error));
			}
			TaskService.ShowErrors ();
		}
	}
}

