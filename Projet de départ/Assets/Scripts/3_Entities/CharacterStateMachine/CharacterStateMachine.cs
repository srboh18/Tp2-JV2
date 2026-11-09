using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts._3_Entities.CharacterStateMachine
{
    public class CharacterStateMachine
    {
        public ICharacterState currentState;

        private readonly Stack<ICharacterState> stateStack =
            new Stack<ICharacterState>();

        private Character character;

        public void Awake(Character character)
        {
            this.character = character;
            PushState(new CharacterWorkingState(character));
        }

        public void Update()
        {

            // On défini l'état courant comme celui au top a chaque update
            currentState = stateStack.Peek();


            // On gère les déchets avant tout
            ManageTrash();

            // On gère les amis 
            ManageGreeting();


            // On le refait parce que parfois il change avec les manages...
            currentState = stateStack.Peek();


            // On gère les besoins du character sauf si est en train de gérer les déchets
            if (currentState is not TrashThrowState &&
                currentState is not TrashPickUpState)
            {
                ManageVitals();
            }

            currentState = stateStack.Peek();

            currentState.Update();
        }
        private void ManageGreeting()
        {
            // Le personnage actuel doit être en déplacement vers le travail.
            if (currentState is not CharacterMovingState movingState ||
                !movingState.IsMovingForWork)
                return;

            Character friend = character.Blackboard.LastFriendSeen;

            if (friend == null)
                return;

            // L'ami doit également être disponible.
            if (friend.StateMachine.currentState is not CharacterMovingState)
                return;

            // Éviter les salutations simultanées.
            if (character.IsGreetingCharacter() || friend.IsGreetingCharacter())
                return;

            PushState(new GreetFriendState(character, friend));

            character.Blackboard.LastFriendSeen = null;
        }
        private void ManageTrash()
        {
            // Ramasser un déchet aperçu pendant un déplacement vers le travail.
            if (currentState is CharacterMovingState movingState &&
                movingState.IsMovingForWork &&
                character.TrashBehaviour == CityCharacterTrashBehaviour.PickUp &&
                character.Blackboard.LastTrashSeen != null)
            {
                PushState(new TrashPickUpState(
                    character,
                    character.Blackboard.LastTrashSeen
                ));
            }
            // Lancer un déchet.
            else if (currentState is CharacterMovingState movingStateThrow &&
                     movingStateThrow.IsMovingForWork &&
                     character.TrashBehaviour == CityCharacterTrashBehaviour.Throw &&
                     character.ShouldThrowTrash)
            {
                PushState(new TrashThrowState(character));
            }
        }

        private void ManageVitals()
        {
            CharacterVitals vitals = character.Vitals;

            // Un déplacement lié à un besoin ou un déchet doit se terminer.
            if (currentState is CharacterMovingState movingState &&
                !movingState.IsMovingForWork)
            {
                return;
            }

            // Ne pas interrompre un besoin déjà en cours.
            if (currentState is TrashThrowState ||
                currentState is TrashPickUpState ||
                currentState is GreetFriendState ||
                currentState is CharacterEatState ||
                currentState is CharacterSleepState ||
                currentState is CharacterSocializeState ||
                currentState is GreetFriendState)
            {
                return;
            }

            // En ordre de priorité ( ce qui descend le plus vite pour éviter une mort)
            if (vitals.IsHungerAboveThreshold)
            {
                PushState(new CharacterEatState(character));
            }
            else if (vitals.IsSleepinessAboveThreshold)
            {
                PushState(new CharacterSleepState(character));
            }
            else if (vitals.IsLonelinessAboveThreshold)
            {
                PushState(new CharacterSocializeState(character));
            }
        }

        public void PushState(ICharacterState newState)
        {
            if (newState == null)
                return;

            if (stateStack.Count > 0)
            {
                stateStack.Peek().Exit();
            }

            stateStack.Push(newState);
            currentState = newState;
            newState.Enter();
        }

        public void PopState()
        {

            stateStack.Peek().Exit();
            stateStack.Pop();

            currentState = stateStack.Peek();
            currentState.Enter();
        }
    }
}