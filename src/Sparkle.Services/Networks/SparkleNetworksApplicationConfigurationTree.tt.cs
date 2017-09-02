﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a T4 template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// solutionPath:            <solution>
// projectPath:             <solution>\Sparkle.Services
// projectName:             Sparkle.Services

    
// className:               SparkleNetworksApplicationConfigurationTree
// nameSpace:               Sparkle.Services.Networks
// xmlPathInProject:        Networks\SparkleNetworksApplicationConfigurationTree.xml

// codegen: XML file found
// codegen: read 1 entries
// codegen: Configuration keys: 1.
// codegen: everything is ready. now generating code.

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Sparkle.Infrastructure.Data;

namespace Sparkle.Services.Networks {
  [GeneratedCode("t4", "1.0.0.0")]
  public partial class SparkleNetworksApplicationConfigurationTree {
    private readonly IDictionary<string, AppConfigurationEntry> values;

    internal SparkleNetworksApplicationConfigurationTree(IDictionary<string, AppConfigurationEntry> values) {
      this.values = values;
    }

    public ResourcesClass Resources {
      get { return _ResourcesClass ?? (_ResourcesClass = new ResourcesClass(this.values)); }
    }
    public class ResourcesClass {
      private readonly IDictionary<string, AppConfigurationEntry> values;

      internal ResourcesClass(IDictionary<string, AppConfigurationEntry> values) {
        this.values = values;
      }

      public PotsClass Pots {
        get { return _PotsClass ?? (_PotsClass = new PotsClass(this.values)); }
      }
      public class PotsClass {
        private readonly IDictionary<string, AppConfigurationEntry> values;

        internal PotsClass(IDictionary<string, AppConfigurationEntry> values) {
          this.values = values;
        }

        /// <summary>
        /// 
        /// </summary>
        public string BaseDirectoryPath {
          get { return SparkleNetworksApplicationConfigurationTree.GetValue<string>(this.values, "Resources.Pots.BaseDirectoryPath"); }
        }

      }
      private PotsClass _PotsClass;

    }
    private ResourcesClass _ResourcesClass;

    private static readonly string[] configKeys = new string[] {
      "Resources.Pots.BaseDirectoryPath",
    };

    private static readonly string[] configKeyTypes = new string[] {
      "System.String",
    };

    private static readonly string[] configKeyValues = new string[] {
      null,
    };

    public static string[] ConfigKeys {
      get { return configKeys.ToArray(); }
    }

    public static string[] ConfigKeyTypes {
      get { return configKeyTypes.ToArray(); }
    }

    public static string[] ConfigKeyValues {
      get { return configKeyValues.ToArray(); }
    }
  }
}


