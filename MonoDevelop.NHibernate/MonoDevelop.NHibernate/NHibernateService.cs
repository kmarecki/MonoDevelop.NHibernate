// 
// NHibernateService.cs
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
using System.Collections.Generic;

using MonoDevelop.Core.Serialization;
using MonoDevelop.Ide;
using MonoDevelop.Projects;

namespace MonoDevelop.NHibernate
{
	public static class NHibernateService
	{
		static Dictionary<DotNetProject, NHibernateProject> projects;
		
		static NHibernateService ()
		{
			projects = new Dictionary<DotNetProject, NHibernateProject> ();
		}
		
		public static NHibernateProject CreateNHibernateProject (DotNetProject project)
		{
			NHibernateProject nhproject = GetNHibernateProject (project);
			nhproject.Create ();
			
			IdeApp.ProjectOperations.Save (project);
			
			return nhproject;
		}
		
		public static NHibernateProject GetNHibernateProject (DotNetProject project)
		{
			if (!projects.ContainsKey (project)) {
				NHibernateProject nhproject = new NHibernateProject (project);
//				if (nhproject == null) {
//					string msg = string.Format ("There is no NHibernate project for {0}", project.Name);
//					throw new NullReferenceException (msg);
//				}
				projects.Add (project, nhproject);
			}
			return projects [project];
		}
		
		public static bool HasNHibernateProject (DotNetProject project)
		{
			NHibernateProject nhproject = GetNHibernateProject (project);
			return nhproject.Exists;
		}
	}
}

