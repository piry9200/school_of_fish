using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fish_controller : MonoBehaviour
{
    public Fish fish1 = new Fish();
    public Fish fish2 = new Fish();

    public Fish getFish1(){
        return this.fish1;
    }

    public Fish getFish2(){
        return this.fish2;
    }
    
    // Start is called before the first frame update
    
}
