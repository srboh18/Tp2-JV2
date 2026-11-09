using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts._3_Entities.CharacterStateMachine
{
    public class CharacterSocializeState : ICharacterState
    {
        private readonly Character character;

        public CharacterSocializeState(Character character)
        {
            this.character = character;
        }

        public string StateName => "Socialise";

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
            if (character.Vitals.IsLonelinessBelowTarget)
            {
                //  Si le character est nourri complètement il revient a son état précédent
                character.stateMachine.PopState();
                return;
            }

            if (IsInSocialBuilding())
            {
                character.Vitals.LowerLoneliness();
            }
            else
            {
                Building[] socialBuildings = Finder.GameController.CityObjects.SocialBuildings;

                // Filtrer les bâtiments disponibles.
                Building[] availableBuildings = socialBuildings
                    .Where(building => !IsBuildingOccupied(building))
                    .ToArray();

                // Choisir un bâtiment aléatoirement.
                Building socialBuilding = availableBuildings[
                    Random.Range(0, availableBuildings.Length)
                ];

                character.stateMachine.PushState(
                    new CharacterMovingState(character, socialBuilding)
                );
            }
        }

        private bool IsBuildingOccupied(Building building)
        {
            Character[] characters = Finder.GameController.CityObjects.Characters;

            foreach (Character other in characters)
            {
                if (other == character)
                    continue;

                // Vérifier si un autre personnage invisible occupe le bâtiment.
                if (!other.IsAvailable && other.IsCloseTo(building))
                    return true;
            }

            return false;
        }

        private bool IsInSocialBuilding()
        {
            Building[] socialBuildings = Finder.GameController.CityObjects.SocialBuildings;

            foreach (Building socialBuilding in socialBuildings)
            {
                if (character.IsCloseTo(socialBuilding))
                    return true;
            }

            return false;
        }
    }
}
