using PLAYERTWO.PlatformerProject;
using UnityEngine;

public class FootstepEventRelay : MonoBehaviour
{
    public PlayerFootsteps footsteps;

    public void Footstep()
    {
        if (footsteps != null)
            footsteps.Footstep();
    }
}
