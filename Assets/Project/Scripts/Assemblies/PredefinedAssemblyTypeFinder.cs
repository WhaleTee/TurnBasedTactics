using System;
using System.Collections.Generic;
using System.Linq;

namespace WhaleTee.Assemblies {
  public static class PredefinedAssemblyTypeFinder {
    enum AssemblyType {
      AssemblyCSharp,
      AssemblyCSharpEditor,
      AssemblyCSharpEditorFirstPass,
      AssemblyCSharpFirstPass
    }

    static AssemblyType? GetAssemblyType(string assemblyName) {
      return assemblyName switch {
               "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
               "Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
               "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
               "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
               var _ => null
             };
    }

    static void AddTypesFromAssemblyByInterface(IEnumerable<Type> assemblyTypes, Type interfaceType, IList<Type> results) {
      var types = AddTypesFromAssemblyByPredicate(assemblyTypes, type => type != interfaceType && interfaceType.IsAssignableFrom(type));

      foreach (var type in types) {
        results.Add(type);
      }
    }

    static void AddTypesFromAssemblyByAttributes(IEnumerable<Type> assemblyTypes, IEnumerable<Type> attributes, IList<Type> results) {
      var types = AddTypesFromAssemblyByPredicate(
        assemblyTypes,
        type => attributes.Any(attribute => type.GetCustomAttributes(attribute, true).Length > 0)
      );

      foreach (var type in types) {
        results.Add(type);
      }
    }

    static IEnumerable<Type> AddTypesFromAssemblyByPredicate(IEnumerable<Type> assemblyTypes, Predicate<Type> predicate) {
      return assemblyTypes?.Where(predicate.Invoke).ToList() ?? new List<Type>();
    }

    public static List<Type> GetTypes(Type interfaceType) {
      var assemblies = AppDomain.CurrentDomain.GetAssemblies();

      var assemblyTypes = new Dictionary<AssemblyType, Type[]>();
      var types = new List<Type>();

      foreach (var assembly in assemblies) {
        var assemblyType = GetAssemblyType(assembly.GetName().Name);

        if (assemblyType != null) {
          assemblyTypes.Add((AssemblyType)assemblyType, assembly.GetTypes());
        }
      }

      assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharp, out var assemblyCSharpTypes);
      AddTypesFromAssemblyByInterface(assemblyCSharpTypes, interfaceType, types);

      assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharpFirstPass, out var assemblyCSharpFirstPassTypes);
      AddTypesFromAssemblyByInterface(assemblyCSharpFirstPassTypes, interfaceType, types);

      return types;
    }

    public static List<Type> GetTypesWithAttributes(ICollection<Type> attributes) {
      var assemblies = AppDomain.CurrentDomain.GetAssemblies();

      var assemblyTypes = new Dictionary<AssemblyType, Type[]>();
      var types = new List<Type>();

      foreach (var assembly in assemblies) {
        var assemblyType = GetAssemblyType(assembly.GetName().Name);

        if (assemblyType != null) {
          assemblyTypes.Add((AssemblyType)assemblyType, assembly.GetTypes());
        }
      }

      assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharp, out var assemblyCSharpTypes);
      AddTypesFromAssemblyByAttributes(assemblyCSharpTypes, attributes, types);

      assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharpFirstPass, out var assemblyCSharpFirstPassTypes);
      AddTypesFromAssemblyByAttributes(assemblyCSharpFirstPassTypes, attributes, types);

      return types;
    }

    public static List<Type> GetTypesWithAttributes(Type attribute) {
      var assemblies = AppDomain.CurrentDomain.GetAssemblies();

      var assemblyTypes = new Dictionary<AssemblyType, Type[]>();
      var types = new List<Type>();

      foreach (var assembly in assemblies) {
        var assemblyType = GetAssemblyType(assembly.GetName().Name);

        if (assemblyType != null) {
          assemblyTypes.Add((AssemblyType)assemblyType, assembly.GetTypes());
        }
      }

      var attributeList = new List<Type> { attribute };

      assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharp, out var assemblyCSharpTypes);
      AddTypesFromAssemblyByAttributes(assemblyCSharpTypes, attributeList, types);

      assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharpFirstPass, out var assemblyCSharpFirstPassTypes);
      AddTypesFromAssemblyByAttributes(assemblyCSharpFirstPassTypes, attributeList, types);
      
      return types;
    }
  }
}