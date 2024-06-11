using System.Reflection;

namespace gbemu.cpu;

public partial class Cpu
{
    private Dictionary<InType, Action> _FetchFuncMap;
    
    private void InitializeFuncMap()
    {
        _ExecFuncMap = new Dictionary<InType, Action>();

        // 获取当前类的所有方法
        var methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var method in methods)
        {
            if (method.Name.StartsWith("Execute") && method.ReturnType == typeof(void) &&
                method.GetParameters().Length == 0)
            {
                // 从方法名中提取 IN_XXX
                var inType = method.Name.Replace("Execute", "IN_");
                var inTypeEnum = (InType)Enum.Parse(typeof(InType), inType);

                // 添加到字典
                _ExecFuncMap.Add(inTypeEnum, method.CreateDelegate<Action>(this));
                Console.WriteLine($"Add ExecFunc {method.Name}");
            }
        }
    }
}