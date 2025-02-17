﻿//
// BIM IFC library: this library works with Autodesk(R) Revit(R) to export IFC files containing model geometry.
// Copyright (C) 2012  Autodesk, Inc.
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.IFC;
using Revit.IFC.Common.Utility;

namespace Revit.IFC.Export.Utility
{
   /// <summary>
   /// Used to keep a cache of the element ids mapping to a IfcMaterial___Set handle (includes IfcMaterialLayerSet, IfcMaterialProfileSet, IfcMaterialConstituentSet in IFC4).
   /// </summary>
   public class MaterialSetCache
   {
      /// <summary>
      /// The dictionary mapping from an ElementId to an IfcMaterialLayerSet handle. 
      /// </summary>
      private Dictionary<ElementId, IFCAnyHandle> m_ElementIdToMatLayerSetDictionary = new Dictionary<ElementId, IFCAnyHandle>();

      /// <summary>
      /// The dictionary mapping from an ElementId to an IfcMaterialProfileSet handle. 
      /// </summary>
      private Dictionary<ElementId, IFCAnyHandle> m_ElementIdToMatProfileSetDictionary = new Dictionary<ElementId, IFCAnyHandle>();

      /// <summary>
      /// The dictionary mapping from an ElementId to an IfcMaterialConstituentSet handle. 
      /// </summary>
      //private Dictionary<ElementId, IFCAnyHandle> m_ElementIdToMatConstituentSetDictionary = new Dictionary<ElementId, IFCAnyHandle>();
      private Dictionary<ElementId, MaterialLayerSetInfo> m_ElementIdToMaterialLayerSetInfo = new Dictionary<ElementId, MaterialLayerSetInfo>();

      /// <summary>
      /// The dictionary mapping from an ElementId to a primary IfcMaterial handle. 
      /// </summary>
      private Dictionary<ElementId, IFCAnyHandle> m_ElementIdToMaterialHndDictionary = new Dictionary<ElementId, IFCAnyHandle>();

      /// <summary>
      /// The dictionary mapping from an ElementId to an IfcMaterialConstituentSet
      /// </summary>
      private Dictionary<ElementId, IFCAnyHandle> m_ElementIdToMaterialConstituentSetDict = new Dictionary<ElementId, IFCAnyHandle>();

      /// <summary>
      /// Finds the IfcMaterialLayerSet handle from the dictionary.
      /// </summary>
      /// <param name="id">The element id.</param>
      /// <returns>The IfcMaterialLayerSet handle.</returns>
      public IFCAnyHandle FindLayerSet(ElementId id)
      {
         IFCAnyHandle handle;
         if (m_ElementIdToMatLayerSetDictionary.TryGetValue(id, out handle))
         {
            return handle;
         }
         return null;
      }

      /// <summary>
      /// Finds the IfcMaterialProfileSet handle from the dictionary.
      /// </summary>
      /// <param name="id">The element id.</param>
      /// <returns>The IfcMaterialProfileSet handle.</returns>
      public IFCAnyHandle FindProfileSet(ElementId id)
      {
         IFCAnyHandle handle;
         if (m_ElementIdToMatProfileSetDictionary.TryGetValue(id, out handle))
         {
            return handle;
         }
         return null;
      }

      /// <summary>
      /// Find MaterialLayerSetInfo from the dictionary cache
      /// </summary>
      /// <param name="id">The element id</param>
      /// <returns>the MaterialLayerSetInfo</returns>
      public MaterialLayerSetInfo FindMaterialLayerSetInfo(ElementId id)
      {
         MaterialLayerSetInfo mlsInfo;
         if (m_ElementIdToMaterialLayerSetInfo.TryGetValue(id, out mlsInfo))
         {
            return mlsInfo;
         }
         return null;
      }

      /// <summary>
      /// Adds the IfcMaterialLayerSet handle to the dictionary.
      /// </summary>
      /// <param name="elementId">The element elementId.</param>
      /// <param name="handle">The IfcMaterialLayerSet handle.</param>
      public void RegisterLayerSet(ElementId elementId, IFCAnyHandle handle, MaterialLayerSetInfo mlsInfo = null)
      {
         if (m_ElementIdToMatLayerSetDictionary.ContainsKey(elementId))
            return;

         m_ElementIdToMatLayerSetDictionary[elementId] = handle;

         if (m_ElementIdToMaterialLayerSetInfo.ContainsKey(elementId) || mlsInfo == null)
            return;

         if ((ExporterCacheManager.ExportOptionsCache.ExportAs4 && IFCAnyHandleUtil.IsTypeOf(handle, Common.Enums.IFCEntityType.IfcMaterialConstituentSet))
            || (ExporterCacheManager.ExportOptionsCache.ExportAs2x3 && IFCAnyHandleUtil.IsTypeOf(handle, Common.Enums.IFCEntityType.IfcMaterialLayerSet)))
            m_ElementIdToMaterialLayerSetInfo.Add(elementId, mlsInfo);
      }

      /// <summary>
      /// Removes element id and associated info from m_ElementIdToMaterialLayerSetInfo and m_ElementIdToMatLayerSetDictionary dictionaries.
      /// </summary>
      /// <param name="elementId">The element elementId.</param>
      public void UnregisterLayerSet(ElementId elementId)
      {
         m_ElementIdToMaterialLayerSetInfo.Remove(elementId);
         m_ElementIdToMatLayerSetDictionary.Remove(elementId);
      }

      /// <summary>
      /// Adds the IfcMaterialProfileSet handle to the dictionary.
      /// </summary>
      /// <param name="elementId">The element elementId.</param>
      /// <param name="handle">The IfcMaterialLayerSet handle.</param>
      public void RegisterProfileSet(ElementId elementId, IFCAnyHandle handle)
      {
         if (m_ElementIdToMatProfileSetDictionary.ContainsKey(elementId))
            return;

         m_ElementIdToMatProfileSetDictionary[elementId] = handle;
      }

      /// <summary>
      /// Finds the primary IfcMaterial handle from the dictionary.
      /// </summary>
      /// <param name="id">The element id.</param>
      /// <returns>The IfcMaterial handle.</returns>
      public IFCAnyHandle FindPrimaryMaterialHnd(ElementId id)
      {
         IFCAnyHandle handle;
         if (m_ElementIdToMaterialHndDictionary.TryGetValue(id, out handle))
         {
            return handle;
         }
         return null;
      }

      /// <summary>
      /// Adds the primary IfcMaterial handle to the dictionary.
      /// </summary>
      /// <param name="elementId">The element elementId.</param>
      /// <param name="handle">The IfcMaterial handle.</param>
      public void RegisterPrimaryMaterialHnd(ElementId elementId, IFCAnyHandle handle)
      {
         if (IFCAnyHandleUtil.IsNullOrHasNoValue(handle))
            return;

         if (m_ElementIdToMaterialHndDictionary.ContainsKey(elementId))
            return;

         m_ElementIdToMaterialHndDictionary[elementId] = handle;
      }

      /// <summary>
      /// Removes element id and associated info from m_ElementIdToMaterialHndDictionary dictionary.
      /// </summary>
      /// <param name="elementId">The element elementId.</param>
      /// <param name="handle">The IfcMaterial handle.</param>
      public void UnregisterPrimaryMaterialHnd(ElementId elementId)
      {
         m_ElementIdToMaterialHndDictionary.Remove(elementId);
      }

      /// <summary>
      /// Finds the IfcMaterialConstituentSet handle from the dictionary.
      /// </summary>
      /// <param name="id">The element id.</param>
      /// <returns>The IfcMaterial handle.</returns>
      public IFCAnyHandle FindConstituentSetHnd(ElementId id)
      {
         IFCAnyHandle handle;
         if (m_ElementIdToMaterialConstituentSetDict.TryGetValue(id, out handle))
         {
            return handle;
         }
         return null;
      }

      /// <summary>
      /// Adds the IfcMaterialConstituentSet handle to the dictionary.
      /// </summary>
      /// <param name="elementId">The element elementId.</param>
      /// <param name="handle">The IfcMaterialConstituentSet handle.</param>
      public void RegisterConstituentSetHnd(ElementId elementId, IFCAnyHandle handle)
      {
         if (IFCAnyHandleUtil.IsNullOrHasNoValue(handle))
            return;

         if (m_ElementIdToMaterialConstituentSetDict.ContainsKey(elementId))
            return;

         m_ElementIdToMaterialConstituentSetDict[elementId] = handle;
      }
   }
}