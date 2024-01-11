// PatrolTypes.cs
namespace SpaceShooterFinal
{
    // Enum for different types of patrol behavior
    public enum PatrolType
    {
        Stationary,          // Enemy remains in a fixed position
        Jitter,              // Enemy makes small, random movements
        SmoothAcrossScreen,  // Enemy moves smoothly across the screen
        SmoothSideToSide,    // Enemy moves smoothly from side to side
        LethargicCreep,      // Enemy moves slowly in a creeping manner
        GlacialDrift         // Enemy drifts very slowly, like a glacier
    }
}

