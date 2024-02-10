using System.Threading.Tasks;

namespace CustomWorkflowIOData
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var blidget = new CustomWorkflowIODataBlidget();
            await blidget.ExecuteAsync();
        }
    }
}
