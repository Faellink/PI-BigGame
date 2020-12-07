using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHackThing : MonoBehaviour
{
    public ZombieAI zombie;
    public void StopStun()
    {
        zombie.StopStun();
    }

    public void FinishAttack()
    {
        zombie.FinishAttack();
    }
    public void EndAttack()
    {
        zombie.EndAttack();
    }
}
