// 
//  ProjectCommandHandler.cs
//  
//  Author:
//       Krzysztof Marecki
// 
//  Copyright (c) 2010 KrzysztofMarecki
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

using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;
using MonoDevelop.Projects;

namespace MonoDevelop.NHibernate.NodeBuilders
{
	public class ProjectCommandHandler : NodeCommandHandler
	{
		[CommandHandler (MonoDevelop.NHibernate.Commands.CreateNHProject)]
		public void OnCreateNHProject ()
		{
			DotNetProject project = (DotNetProject) CurrentNode.GetParentDataItem (typeof(DotNetProject), true);
			NHService.CreateNHibernateProject (project);
		}
		
		[CommandUpdateHandler (MonoDevelop.NHibernate.Commands.CreateNHProject)]
		public void UpdateCreateNHProject (CommandInfo cinfo)
		{
			DotNetProject project = (DotNetProject) CurrentNode.GetParentDataItem (typeof(DotNetProject), true);
			cinfo.Visible = !NHService.HasNHibernateProject (project);
		}
	}
}

