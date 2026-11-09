using UnityEngine;

namespace Assets.Scripts._3_Entities.CharacterStateMachine
{
    public class TrashThrowState : ICharacterState
    {
        private readonly Character character;

        public TrashThrowState(Character character)
        {
            this.character = character;
        }

        public string StateName => "Lance un déchet";

        public void Enter()
        {
            character.ThrowTrash();
        }

        public void Exit()
        {
        }

        public void Update()
        {
            // Attendre que l'animation de lancer soit terminée.

            // Les besoins augmentent
            character.Vitals.RaiseHunger();
            character.Vitals.RaiseLoneliness();
            character.Vitals.RaiseSleepiness();

            if (!character.IsThrowingTrash())
            {
                character.stateMachine.PopState();
            }
        }
    }
}