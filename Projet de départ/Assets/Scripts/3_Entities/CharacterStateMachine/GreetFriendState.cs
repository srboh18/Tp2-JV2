using Assets.Scripts._3_Entities.CharacterStateMachine;

public class GreetFriendState : ICharacterState
{
    private readonly Character character;
    private readonly Character friend;

    public GreetFriendState(Character character, Character friend)
    {
        this.character = character;
        this.friend = friend;
    }

    public string StateName => "Salue un ami";

    public void Enter()
    {
        character.GreetCharacter(friend);
    }

    public void Update()
    {
        character.Vitals.LowerLoneliness();
        if (!character.IsGreetingCharacter())
        {
            character.StateMachine.PopState();
        }
    }

    public void Exit()
    {
    }
}