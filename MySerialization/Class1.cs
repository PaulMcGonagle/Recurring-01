using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using IContainer = System.ComponentModel.IContainer;

namespace MySerialization
{
    public class Class1
    {
    }



    public class AutofacContractResolver : DefaultContractResolver
    {
        private readonly IContainer _container;

        public AutofacContractResolver(IContainer container)
        {
            _container = container;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            JsonObjectContract contract = base.CreateObjectContract(objectType);

            // use Autofac to create types that have been registered with it
            if (_container.IsRegistered(objectType))
            {
                contract.DefaultCreator = () => _container.Resolve(objectType);
            }

            return contract;
        }
    }

    public object MyDeserialize(string json)
    {
        ContainerBuilder builder = new ContainerBuilder();
        builder.RegisterType<TaskRepository>().As<ITaskRepository>();
        builder.RegisterType<TaskController>();
        builder.Register(c => new LogService(new DateTime(2000, 12, 12))).As<ILogger>();
        
        IContainer container = builder.Build();
        
        AutofacContractResolver contractResolver = new AutofacContractResolver(container);
        
        string json = @"{
          'Logger': {
            'Level':'Debug'
          }
        }";
        
        // ITaskRespository and ILogger constructor parameters are injected by Autofac 
        TaskController controller = JsonConvert.DeserializeObject<TaskController>(json, new JsonSerializerSettings
        {
            ContractResolver = contractResolver
        });
        
        Console.WriteLine(controller.Repository.GetType().Name);
        // TaskRepository    
    }
}
