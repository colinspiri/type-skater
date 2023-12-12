using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Trick : Word {
    [Header("Trick Fields")] 
    public int trickScore;
    public List<Player.State> availableInStates;
    
    public override void Complete() {
        base.Complete();
    }
}