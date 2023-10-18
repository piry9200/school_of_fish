using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class Fish
{
    public int fish_id; //魚種を示す
    public float ray_long;
    public float visibility_radi; //個体の視野の半径
    public float keep_distance; //維持すべき距離
    public float max_speed;
    public float min_speed;
    public float target_weight;
    public float keep_dist_weight;
    public float follow_weight;
    public float ahead_center_weight;
    public float  avoid_obst_weight;
}
