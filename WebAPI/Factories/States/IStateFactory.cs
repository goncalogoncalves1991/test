using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Factories.States
{
    
    public interface IStateFactory<TModel, TState>
    {
        TState Create(TModel model);

        
    }
   
}
