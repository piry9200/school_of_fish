using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Manager : MonoBehaviour
{
        [SerializeField, Range(2, 50)] public int child_max = 10;
        [SerializeField, Range(0f, 20.0f)] public float ray_long = 2; //正面検知の長さ
        [SerializeField, Range(0f, 100.0f)]public float visibility_radi = 5f; //個体の視野の半径
        public Vector3 flock_center;
        [SerializeField, Range(0.1f, 10f)] public float keep_distance = 1.5f; //他の個体と維持すべき距離
        [SerializeField, Range(1.0f, 50.0f)] public float max_speed = 10f;
        [SerializeField, Range(1.0f, 50.0f)]public float min_speed = 3f;
        public float target_weight = 1f;
        [SerializeField, Range(0f, 300.0f)] public float keep_dist_weight = 1f;
        [SerializeField, Range(0f, 300.0f)] public float follow_weight = 0.5f;
        [SerializeField, Range(0f, 300.0f)] public float ahead_center_weight = 0.5f;
        [SerializeField, Range(0f, 5000.0f)] public float  avoid_obst_weight = 10f;
        public GameObject child;
        public List<GameObject> children;
        public GameObject option_canvas;
        public Slider SL_CHILDMAX;
        public Slider SL_RAYLONG;
        public Slider SL_VISIRADI;
        public Slider SL_KEEPDIST;
        public Slider SL_MINSPEED;
        public Slider SL_MAXSPEED;
        public Slider SL_KEEPDIST_W;
        public Slider SL_FOLLOW_W;
        public Slider SL_AHEADCENTER_W;
        public Slider SL_AVOIDOBST_W;

        public TextMeshProUGUI TX_CHILDMAX;
        public TextMeshProUGUI TX_RAYLONG;
        public TextMeshProUGUI TX_VISIRADI;
        public TextMeshProUGUI TX_KEEPDIST;
        public TextMeshProUGUI TX_MINSPEED;
        public TextMeshProUGUI TX_MAXSPEED;
        public TextMeshProUGUI TX_KEEPDIST_W;
        public TextMeshProUGUI TX_FOLLOW_W;
        public TextMeshProUGUI TX_AHEADCENTER_W;
        public TextMeshProUGUI TX_AVOIDOBST_W;

        public GameObject Spawner;
    // Start is called before the first frame update
    void Start()
    {
        //オブジェクトを任意の数だけ配置する
        children = new List<GameObject>();
        Debug.Log(children.Count);

        SL_CHILDMAX.value =  child_max;
        SL_RAYLONG.value =  ray_long;
        SL_VISIRADI.value =  visibility_radi;
        SL_KEEPDIST.value =  keep_distance;
        SL_MINSPEED.value =  min_speed;
        SL_MAXSPEED.value =  max_speed;
        SL_KEEPDIST_W.value =  keep_dist_weight;
        SL_FOLLOW_W.value =  follow_weight;
        SL_AHEADCENTER_W.value =  ahead_center_weight;
        SL_AVOIDOBST_W.value =  avoid_obst_weight;
    }

    // Update is called once per frame
    void Update()
    {
        spawn();
        //重心を求める
        //flock_center = culc_center(children);
        //スライダーが表示されているときだけ実行
        if(option_canvas.activeSelf){
            //sliderの値を反映
            
            child_max = (int)SL_CHILDMAX.value;
            ray_long = SL_RAYLONG.value;
            visibility_radi = SL_VISIRADI.value;
            keep_distance = SL_KEEPDIST.value;
            min_speed = SL_MINSPEED.value;
            max_speed = SL_MAXSPEED.value;
            keep_dist_weight = SL_KEEPDIST_W.value;
            follow_weight = SL_FOLLOW_W.value;
            ahead_center_weight = SL_AHEADCENTER_W.value;
            avoid_obst_weight = SL_AVOIDOBST_W.value;

            TX_CHILDMAX.text = "child_max: " + child_max.ToString();
            TX_RAYLONG.text = "ray_long: " + ray_long.ToString();
            TX_VISIRADI.text = "visibility_radi: " + visibility_radi.ToString();
            TX_KEEPDIST.text = "keep_distance: " + keep_distance.ToString();
            TX_MINSPEED.text = "min_speed: " + min_speed.ToString();
            TX_MAXSPEED.text = "max_speed: " + max_speed.ToString();
            TX_KEEPDIST_W.text = "keep_dist_weight: " + keep_dist_weight.ToString();
            TX_FOLLOW_W.text = "follow_weight: " + follow_weight.ToString();
            TX_AHEADCENTER_W.text = "ahead_center_weight: " + ahead_center_weight.ToString();
            TX_AVOIDOBST_W.text = "avoid_obst_weight: " + avoid_obst_weight.ToString();
        }
    }

    private Vector3 culc_center(GameObject[] children){
        Vector3 center = Vector3.zero;
        foreach(GameObject child in this.children){
            Transform child_t = child.transform.GetChild(0);
            GameObject child_ob = child_t.gameObject;
            center += child_ob.transform.position;
        }
        center /= children.Length - 1;
        return center;
    }
/*
    private Vector3 culc_ave_vel(GameObject[] children){
        Vector3 ave_vel = Vector3.zero;
        foreach(GameObject child in this.children){
            Transform child_t = child.transform.GetChild(0);
            GameObject child_ob = child_t.gameObject;
            ave_vel += child_ob.GetComponent<Rigidbody>().velocity;
        }
        ave_vel /= children.Length - 1;
        return ave_vel;
    }
    */

    public void option_button(){
        if(option_canvas.activeSelf){
            option_canvas.SetActive(false);
        }else{
            option_canvas.SetActive(true);
        }
    }

    public void spawn(){
        int child_num = this.children.Count;
        if(child_num < child_max){
            this.children.Add(Instantiate(child));
            this.children[child_num].transform.position = this.Spawner.transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        }else if(child_num > child_max){
            GameObject.Destroy(this.children[child_num-1]);
            this.children.RemoveAt(child_num-1);
        }
    }
}
