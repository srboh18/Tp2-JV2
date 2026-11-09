using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Assets.Scripts._3_Entities.CharacterStateMachine
{
    public class TrashPickUpState : ICharacterState
    {
        private readonly Character character;
        private readonly Trash trash;

        private bool isPickingUp;

        public string StateName => "Ramasse un déchet";

        public TrashPickUpState(Character character, Trash trash)
        {
            this.character = character;
            this.trash = trash;
        }

        public void Enter()
        {
            isPickingUp = false;

            if (trash == null || !trash.IsAvailable)
            {
                character.stateMachine.PopState();
                return;
            }

            HandleToTrash();
        }

        public void Update()
        {
            if (trash == null || !trash.IsAvailable)
            {
                character.stateMachine.PopState();
                return;
            }

            if (isPickingUp)
            {
                if (!character.IsPickingTrash())
                {
                    character.stateMachine.PopState();
                }

                return;
            }


            // Le déplacement empilé doit être terminé avant de reprendre.
            if (character.stateMachine.currentState != this)
                return;

            // Les besoins augmentent
            character.Vitals.RaiseHunger();
            character.Vitals.RaiseLoneliness();
            character.Vitals.RaiseSleepiness();

            HandleToTrash();
        }

        private void HandleToTrash()
        {
            if (!character.IsCloseTo(trash))
            {
                character.stateMachine.PushState(
                    new CharacterMovingState(character, trash)
                );
                return;
            }

            StartPickingUp();
        }

        private void StartPickingUp()
        {
            isPickingUp = true;
            character.CancelNavigate();
            character.PickUpTrash(trash);
        }

        public void Exit()
        {
            character.CancelNavigate();
        }
    }
}