﻿using System;
using System.Collections.Generic;
using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;

using Machine.Specifications.ReSharperRunner.Factories;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  class ContextSpecificationElement : FieldElement
  {
    readonly string _id;

    public ContextSpecificationElement(MSpecUnitTestProvider provider,
                                       PsiModuleManager psiModuleManager,
                                       CacheManager cacheManager,
                                       // ReSharper disable SuggestBaseTypeForParameter
                                       ContextElement context,
                                       // ReSharper restore SuggestBaseTypeForParameter
                                       ProjectModelElementEnvoy project,
                                       IClrTypeName declaringTypeName,
                                       string fieldName,
                                       bool isIgnored)
      : base(
        provider,
        psiModuleManager,
        cacheManager,
        context,
        project,
        declaringTypeName,
        fieldName,
        isIgnored || context.Explicit)
    {
      _id = CreateId(context, fieldName);
    }

    public ContextElement Context
    {
      get { return (ContextElement) Parent; }
    }

    public override string Kind
    {
      get { return "Specification"; }
    }

    public override IEnumerable<UnitTestElementCategory> Categories
    {
      get
      {
        var parent = Parent ?? Context;
        if (parent == null)
        {
          return UnitTestElementCategory.Uncategorized;
        }

        return parent.Categories;
      }
    }

    public override string Id
    {
      get { return _id; }
    }

    public static IUnitTestElement ReadFromXml(XmlElement parent,
                                               IUnitTestElement parentElement,
                                               MSpecUnitTestProvider provider,
                                               ISolution solution
                                               ,
                                               IUnitTestElementManager manager,
                                               PsiModuleManager psiModuleManager,
                                               CacheManager cacheManager
      )
    {
      var projectId = parent.GetAttribute("projectId");
      var project = ProjectUtil.FindProjectElementByPersistentID(solution, projectId) as IProject;
      if (project == null)
      {
        return null;
      }

      var context = parentElement as ContextElement;
      if (context == null)
      {
        return null;
      }

      var typeName = parent.GetAttribute("typeName");
      var methodName = parent.GetAttribute("methodName");
      var isIgnored = bool.Parse(parent.GetAttribute("isIgnored"));

      return ContextSpecificationFactory.GetOrCreateContextSpecification(provider,
                                                                         manager,
                                                                         psiModuleManager,
                                                                         cacheManager,
                                                                         project,
                                                                         context,
                                                                         ProjectModelElementEnvoy.Create(project),
                                                                         new ClrTypeName(typeName),
                                                                         methodName,
                                                                         isIgnored);
    }

    public static string CreateId(ContextElement contextElement, string fieldName)
    {
      return String.Format("{0}.{1}", contextElement.Id, fieldName);
    }
  }
}