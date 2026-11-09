using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.Scripts._3_Entities.CharacterStateMachine
{
    public class CharacterEatState : ICharacterState
    {
        private readonly Character character;
        private readonly GameController controller;
        public CharacterEatState(Character character)
        {
            this.character = character;
            this.controller = Finder.GameController;
        }

        public string StateName => "Mange";

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
            if (character.Vitals.IsHungerBelowTarget)
            {
                //  Si le character est nourri complètement il revient a son état précédent
                character.stateMachine.PopState();
                return;
            }

            if (IsInFoodBuilding())
            {

                character.Vitals.LowerHunger();
            }
            else
            {
                Building[] foodBuildings = controller.CityObjects.FoodBuildings;


                //  On filtre les foodBuildings dispo..
                Building[] availableBuildings = foodBuildings
                    .Where(building =>!IsBuildingOccupied(building))
                    .ToArray();

                Building foodBuilding = availableBuildings[
                    Random.Range(0, availableBuildings.Length)
                ];

                character.stateMachine.PushState(
                    new CharacterMovingState(character, foodBuilding)
                );
            }



        }

        private bool IsInFoodBuilding()
        {
            Building[] foodBuildings = Finder.GameController.CityObjects.FoodBuildings;

            foreach (Building foodBuilding in foodBuildings)
            {
                if (character.IsCloseTo(foodBuilding))
                    return true;
            }

            return false;
        }
        private bool IsBuildingOccupied(Building building)
        {
            Character[] characters = controller.CityObjects.Characters;

            foreach (Character other in characters)
            {


                if (!other.IsAvailable && other.IsCloseTo(building))
                    return true;
            }

            return false;
        }

    }
}
