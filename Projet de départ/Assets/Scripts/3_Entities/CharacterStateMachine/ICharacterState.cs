using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts._3_Entities.CharacterStateMachine
{
    public interface ICharacterState
    {
        string StateName { get; }
        void Enter();
        void Update();
        void Exit();
    }
}
