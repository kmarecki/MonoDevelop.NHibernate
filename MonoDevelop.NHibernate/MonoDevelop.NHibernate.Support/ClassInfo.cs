// 
//  ClassInfo.cs
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
using System.Collections.Generic;
using System.IO;
using NHibernate.Cfg.MappingSchema;

namespace MonoDevelop.NHibernate.Support
{
	public class ClassInfo
	{
		public string Name { get; set; }
		public string Namespace { get; set; }

		public List<PropertyInfo> Properties { get; set; }
		public PropertyInfo Id { get; set; }

		public static ClassInfo LoadFromFile (string fileName, string basePath)
		{
			ClassInfo c = null;
			
			MappingDocumentParser p = new MappingDocumentParser ();
			using (FileStream stream = new FileStream (System.IO.Path.Combine (basePath, fileName), FileMode.Open)) {
				HbmMapping m = p.Parse (stream);
				foreach (object o in m.Items) {
					if (o is HbmClass) {
						HbmClass cl = (HbmClass)o;
						
						c = new ClassInfo ();
						c.Namespace = m.@namespace;
						c.Name = cl.name;
						c.Properties = new List<PropertyInfo> ();
						
						if (cl.Id != null) {
							var id = new PropertyInfo { Name = cl.Id.name, PropertyType = cl.Id.type1 != null ? cl.Id.type1 : "String", RelationType = RelationType.Property, IsNullable = true};
							c.Properties.Add (id);
							c.Id = id;
						}
						
						foreach (object po in cl.Items) {
							if (po is HbmProperty) {
								HbmProperty prop = (HbmProperty)po;
								c.Properties.Add (new PropertyInfo { Name = prop.name, PropertyType = prop.type1 != null ? prop.type1 : "String", RelationType = RelationType.Property, IsNullable = !prop.notnull });
							} else if (po is HbmManyToOne) {
								HbmManyToOne mo = (HbmManyToOne)po;
								c.Properties.Add (new PropertyInfo { Name = mo.name, PropertyType = mo.@class, RelationType = RelationType.ManyToOne, IsNullable = !mo.notnull });
							} else if (po is HbmSet) {
								HbmSet se = (HbmSet)po;
								
								c.Properties.Add (new PropertyInfo { Name = se.name, PropertyType = ((HbmOneToMany)se.Item).@class, RelationType = RelationType.ManyToMany });
							}
						}
					}
				}
			}
			return c;
		}
	}
}

