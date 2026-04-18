using System;
using System.Collections.Generic;
using ZLinq;

namespace WhaleTee.Assemblies {
  public static class AssemblyTypeFinder {
    enum AssemblyType {
      AssemblyCSharp, AssemblyCSharpFirstPass, Undefined
    }

    static AssemblyType GetAssemblyType(string assemblyName) {
      return assemblyName switch {
               "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
               "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
               var _ => AssemblyType.Undefined
             };
    }

    static IEnumerable<Type> FindTypesByPredicate(IEnumerable<Type> assemblyTypes, Predicate<Type> predicate) {
      return assemblyTypes?.Where(predicate.Invoke).ToList() ?? new List<Type>();
    }

    static IEnumerable<Type> FindAssignableTypes(IEnumerable<Type> assemblyTypes, Type interfaceType) {
      return FindTypesByPredicate(assemblyTypes, type => type != interfaceType && interfaceType.IsAssignableFrom(type));
    }

    static IEnumerable<Type> AddTypesByAttributes(IEnumerable<Type> assemblyTypes, IEnumerable<Type> attributes) {
      return FindTypesByPredicate(
        assemblyTypes,
        type => attributes.Any(attribute => type.GetCustomAttributes(attribute, true).Length > 0)
      );
    }

    public static List<Type> GetAssignableTypes(Type interfaceType) {
      var assemblies = AppDomain.CurrentDomain.GetAssemblies();

      var assemblyTypes = new Dictionary<AssemblyType, Type[]>();
      var types = new List<Type>();

      foreach (var assembly in assemblies) {
        var assemblyType = GetAssemblyType(assembly.GetName().Name);
        assemblyTypes.Add(assemblyType, assembly.GetTypes());
      }

      assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharp, out var assemblyCSharpTypes);
      var foundTypes = FindAssignableTypes(assemblyCSharpTypes, interfaceType);
      types.AddRange(foundTypes);

      assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharpFirstPass, out var assemblyCSharpFirstPassTypes);
      foundTypes = FindAssignableTypes(assemblyCSharpFirstPassTypes, interfaceType);
      types.AddRange(foundTypes);

      return types;
    }

    public static List<Type> GetTypesWithAttributes(ICollection<Type> attributes) {
      var assemblies = AppDomain.CurrentDomain.GetAssemblies();

      var assemblyTypes = new Dictionary<AssemblyType, Type[]>();
      var types = new List<Type>();

      foreach (var assembly in assemblies) {
        var assemblyType = GetAssemblyType(assembly.GetName().Name);
        assemblyTypes.Add(assemblyType, assembly.GetTypes());
      }

      assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharp, out var assemblyCSharpTypes);
      var foundTypes = AddTypesByAttributes(assemblyCSharpTypes, attributes);
      types.AddRange(foundTypes);

      assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharpFirstPass, out var assemblyCSharpFirstPassTypes);
      foundTypes = AddTypesByAttributes(assemblyCSharpFirstPassTypes, attributes);
      types.AddRange(foundTypes);

      return types;
    }

    public static List<Type> GetTypesWithAttribute(Type attribute) {
      return GetTypesWithAttributes(new List<Type> { attribute });
    }
  }
}