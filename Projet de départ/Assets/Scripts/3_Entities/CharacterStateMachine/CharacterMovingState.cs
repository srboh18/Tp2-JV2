using UnityEngine;

namespace Assets.Scripts._3_Entities.CharacterStateMachine
{
    public class CharacterMovingState : ICharacterState
    {
        private readonly Character character;
        private readonly IDestination destination;

        public bool IsMovingForWork { get; private set; }
        public string StateName => "En déplacement";

       public bool IsMovingToBuilding => destination is Building;

        public CharacterMovingState(
            Character character,
            IDestination destination,
            bool isMovingForWork = false)
        {
            this.character = character;
            this.destination = destination;
            IsMovingForWork = isMovingForWork;
        }

        public void Enter()
        {
            character.NavigateTo(destination);
        }

        public void Update()
        {
            if (character.IsCloseTo(destination))
            {
                character.stateMachine.PopState();
            }
            else
            {
                character.Vitals.RaiseHunger();
                character.Vitals.RaiseLoneliness();
                character.Vitals.RaiseSleepiness();
            }
        }

        public void Exit()
        {
            character.CancelNavigate();
        }
    }
}