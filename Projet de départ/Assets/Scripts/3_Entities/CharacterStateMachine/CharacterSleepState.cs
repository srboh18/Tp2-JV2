using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts._3_Entities.CharacterStateMachine
{
    public class CharacterSleepState : ICharacterState
    {

        private readonly Character character;

        public CharacterSleepState(Character character)
        {
            this.character = character;
        }

        public string StateName => "Dort";

        public void Enter()
        {
            // Simule l'entrée dans le bâtiment.
            character.MakeInvisible();
        }

        public void Exit()
        {
            // Simule la sortie du bâtiment.
            character.MakeVisible();
        }

        public void Update()
        {
            if(character.Vitals.Sleepiness <= 0)
            {
                //  Si sommeil a 0 on retourne a l'état précédent 
                character.stateMachine.PopState();
                return;
            }




            Building house = character.Blackboard.House;
            float distance = Vector3.Distance(
                    character.Position,
                    house.Position
                );
            if (distance > 0.5f)
            {
                character.stateMachine.PushState(
                    new CharacterMovingState(character, house)
                );
            }
            else
            {
                // Le personnage est déjà a la maison 
                character.Vitals.LowerSleepiness();

            }
        }
    }
}
