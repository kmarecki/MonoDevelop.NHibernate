// 
//  NHibernateCommandHandler.cs
//  
//  Author:
//       Krzysztof Marecki <marecki.krzysztof@gmail.com>
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
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide.Gui.Components;

using MonoDevelop.NHibernate.Dialogs;
	
namespace MonoDevelop.NHibernate.NodeBuilders
{
	public class ProjectFolderCommandHandler : NodeCommandHandler
	{
		[CommandHandler (MonoDevelop.NHibernate.Commands.AddNewHbmFile)]
		public void AddNewHbmFile()
		{
			var dialog = new NewHbmFileDialog ();
			dialog.Run ();
		}
	}
}

