using UnityEngine;

namespace Assets.Scripts._3_Entities.CharacterStateMachine
{
    public class CharacterWorkingState : ICharacterState
    {
        private readonly Character character;

        public string StateName => "Travaille";

        public CharacterWorkingState(Character character)
        {
            this.character = character;
        }

        public void Enter()
        {
            // Simule l'entrée dans le bâtiment.
            character.MakeInvisible();
        }

        public void Update()
        {

            // Si un autre état a été empilé, on ne poursuit pas le travail.
            if (character.stateMachine.currentState != this)
                return;

            HandleToWork();
        }

       

        private void HandleToWork()
        {
            Building workplace = character.Blackboard.Workplace;

            if (!character.IsCloseTo(workplace))
            {
                character.stateMachine.PushState(
                    new CharacterMovingState(character, workplace,true)
                );
                return;
            }

            // Le personnage est au travail : ses besoins augmentent.
            character.Vitals.RaiseHunger();
            character.Vitals.RaiseLoneliness();
            character.Vitals.RaiseSleepiness();
        }

        public void Exit()
        {
            // Simule la sortie du bâtiment.
            character.MakeVisible();
        }
    }
}