using UnityEngine;

// Finder.
//
// Accès aux objets globaux. Vous n'avez pas à toucher cette classe pour le travail.
public static class Finder
{
    private static GameController gameController;
    private static EventChannels eventChannels;
    private static OrbitCamera camera;
    
    public static GameController GameController
    {
        get
        {
            if (gameController == null)
                gameController = FindWithTag<GameController>("GameController");
            return gameController;
        }
    }
    
    public static EventChannels EventChannels
    {
        get
        {
            if (eventChannels == null)
                eventChannels = FindWithTag<EventChannels>("GameController");;
            return eventChannels;
        }
    }
    
    public static OrbitCamera Camera
    {
        get
        {
            if (camera == null)
                camera = FindWithTag<OrbitCamera>("VirtualCamera");;
            return camera;
        }
    }
    
    private static T FindWithTag<T>(string tag) where T : class
    {
        var gameObject = GameObject.FindWithTag(tag);
        if (gameObject == null) return null;
        return gameObject.GetComponent<T>();
    }
}