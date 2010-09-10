// 
//  NHibernateBuildExtension.cs
//  
//  Author:
//       Krzysztof Marecki
// 
//  Copyright (c) 2010 Krzysztof Marecki
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
using System;

using MonoDevelop.Core;
using MonoDevelop.Projects;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.ProgressMonitoring;

namespace MonoDevelop.NHibernate
{
	public class NHibernateBuildExtension : ProjectServiceExtension
	{
		public override void Save (IProgressMonitor monitor, SolutionEntityItem item)
		{
			CheckNHibernateProject (item);
			base.Save (monitor, item);
		}
		
		protected override BuildResult Compile (IProgressMonitor monitor, SolutionEntityItem item, BuildData buildData)
		{
			CheckNHibernateProject (item);
			return base.Compile (monitor, item, buildData);
		}
		
		void CheckNHibernateProject (SolutionEntityItem item)
		{
			DotNetProject project = item as DotNetProject;
			if (project != null) {
				if (NHibernateService.HasNHibernateProject (project)) {
					NHibernateProject nhproject = NHibernateService.GetNHibernateProject (project);
					nhproject.CheckAndGenerateCodeFiles ();
				}
			}
		}
	}
}

