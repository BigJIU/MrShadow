using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class ShadowManager : MonoBehaviour
{
    private List<Transform> activateLightList;
    private Dictionary<Transform, Transform> reflsDic;

    private Dictionary<GameObject, GameObject> refslDic;
    //Instance 
    private static ShadowManager Instance;
    public static ShadowManager getInstance { get { return Instance; } }

    public GameObject shadowPrefab;
    public Transform LightObjects;
    public List<Transform> IndividualLightObjects;
    public GameObject pointLight;
    //public List<Transform> OtherPlayer;
    //Specialized Player Due to the prefab need for shadow
    public GameObject Player;
    public GameObject PlayerShadow;
    private float canvasScale = 100f;
    private Vector3 lightPosi;

    private Dictionary<Transform, List<Vector2[]>> normalVertices;

    [Header("ShadowRelated Parameter")] 
    public float defaultOffsetX = 0f;
    public float defaultOffsetY = 1f;
    public float defaultXScale = 1f;
    public float defaultYScale = 1f;

    public void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        reflsDic = new Dictionary<Transform, Transform>();
        refslDic = new Dictionary<GameObject, GameObject>();
        activateLightList = new List<Transform>();
        normalVertices = new Dictionary<Transform, List<Vector2[]>>();
        //Light, Shadow
        for (int i = 0; i < LightObjects.childCount; i++)
        {
            createShadow(LightObjects.GetChild(i));
        }

        for (int i = 0; i < IndividualLightObjects.Count; i++)
        {
            createShadow(IndividualLightObjects[i]);
        }
        reflsDic.Add(Player.transform,PlayerShadow.transform);
        refslDic.Add(PlayerShadow,Player);
        addNormalVertices(PlayerShadow.transform,PlayerShadow.GetComponent<PolygonCollider2D>());

    }

    void Start()
    {
        lightPosi = pointLight.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        templateLightMove();
        foreach (Transform light in activateLightList)
        {
            Transform shadowTrans = reflsDic[light].transform;
            shadowTrans.position.Set(light.position.x,shadowTrans.position.y,shadowTrans.position.z);
            shadowTrans.localScale.Set(light.localScale.x,shadowTrans.localScale.y,shadowTrans.localScale.z);
        }
        
        if (pointLight.transform.position != lightPosi||Input.GetAxis("Horizontal")!=0)//Input.GetKeyDown(KeyCode.O))
        {
            foreach (Transform light in activateLightList)
            {
                updateShadow(light);
            }

            lightPosi = pointLight.transform.position;
        }

    }

    private void templateLightMove()
    {
        float movespeed = 1f;
        if (Input.GetKey(KeyCode.W))//无脑的移动测试
        {
            pointLight.transform.Translate(Vector3.up * movespeed * Time.deltaTime);         
        }
 
        if (Input.GetKey(KeyCode.S))
        {
            pointLight.transform.Translate(Vector3.down * movespeed * Time.deltaTime);
        
        }
 
        if (Input.GetKey(KeyCode.A) )
        {
            pointLight.transform.Translate(Vector3.left * movespeed * Time.deltaTime);
           
        }
 
        if (Input.GetKey(KeyCode.D))
        {
            pointLight.transform.Translate(Vector3.right * movespeed * Time.deltaTime);
            
        }
    }

    public void createShadow(Transform lightTransform)
    {
        GameObject tmpShadow = Instantiate(shadowPrefab, transform);
        tmpShadow.name = lightTransform.name;
        //reflection offset can be added here!
        tmpShadow.transform.position = new Vector3(lightTransform.position.x,-lightTransform.position.y,lightTransform.position.z);
        reflsDic.Add(lightTransform,tmpShadow.transform);
        refslDic.Add(tmpShadow,lightTransform.gameObject);
        Sprite lightSprite = getSprite(lightTransform);
        tmpShadow.GetComponent<ShapeImage>().sprite = lightSprite;
        tmpShadow.GetComponent<RectTransform>().sizeDelta = new Vector2(lightSprite.rect.width,lightSprite.rect.height);
        FuncObject f;
        if (lightTransform.TryGetComponent(out f))
        {
            f.copyComponent(tmpShadow);
        }
        
        createShadowCollider(lightTransform,lightSprite.rect.height/2);
    }

    private Sprite getSprite(Transform lightTransform)
    {
        //return lightTransform.gameObject.GetComponent<SpriteRenderer>().sprite;
        //TODO: Unecessary judege
        return lightTransform.GetComponent<LightObject>().shadowSprite == null?
            lightTransform.gameObject.GetComponent<SpriteRenderer>().sprite:
            lightTransform.GetComponent<LightObject>().shadowSprite ;
    }

    private void createShadowCollider(Transform lightTransform,float offset)
    {
        PolygonCollider2D lightCollider = lightTransform.GetComponent<PolygonCollider2D>();
        PolygonCollider2D shadowCollider2D = reflsDic[lightTransform].GetComponent<PolygonCollider2D>();
        shadowCollider2D.pathCount = lightCollider.pathCount;
        //foreach (var ver in vertices) Debug.Log(ver.ToString());
        for (int i = 0; i < lightCollider.pathCount; i++)
        {
            Vector2[] vertices = lightCollider.GetPath(0);
            for (int ver = 0; ver < vertices.Length; ver++)
            {
                vertices[ver] *= new Vector2(canvasScale, canvasScale);
                //vertices[ver] += new Vector2(0, offset);
            }
            shadowCollider2D.SetPath(i,vertices);
        }

        if (!normalVertices.ContainsKey(lightTransform))
        {
            addNormalVertices(lightTransform,shadowCollider2D);
        }
        
        //Debug.Log(tryset);
        //foreach (var ver in vertices) Debug.Log(ver.ToString());
    }


    
    public void updateShadow(Transform lightTransform)
    {
        //calculate transform vector
        float diffX = lightTransform.position.x - pointLight.transform.position.x - defaultOffsetX;
        float diffY = pointLight.transform.position.y - lightTransform.position.y - defaultOffsetY;
        
        Vector2 tranformVector = new Vector2(diffX * defaultXScale, diffY * defaultYScale);
        
        colliderTransform(tranformVector,lightTransform);
        imageTransform(tranformVector,lightTransform);
        //do transform in collider
        //do transform in image
    }

    private void colliderTransform(Vector2 tranformVector,Transform lightTransform)
    {
        
        PolygonCollider2D shadowCollider = reflsDic[lightTransform].GetComponent<PolygonCollider2D>();
        
        //Vector3 boundSize = shadowCollider.bounds.size;
        float ymin = 10000f;
        float ymax = -10000f;
        for (int pi = 0; pi < shadowCollider.pathCount; pi++)
        {
            Vector2[] vertices = shadowCollider.GetPath(pi);
            foreach (var ver in vertices)
            {
                if (ver.y < ymin) ymin = ver.y;
                if (ver.y > ymax) ymax = ver.y;
            }
        }

        float ybounds = ymax - ymin;
        for (int pi = 0; pi < shadowCollider.pathCount; pi++)
        {
            Vector2[] vertices = normalVertices[lightTransform][pi];
            Vector2[] setpath = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                // setpath[i] = new Vector2(vertices[i].x-tranformVector.x*((ybounds-vertices[i].y)/ybounds)*(1+tranformVector.y)*(1+tranformVector.y), 
                //     vertices[i].y*(1+tranformVector.y));//*(1+tranformVector.y)
                setpath[i] = new Vector2(vertices[i].x+(tranformVector.x*(vertices[i].y-ymin)/ybounds) * canvasScale*(1+tranformVector.y)*(1+tranformVector.y), 
                     vertices[i].y*(1+tranformVector.y));//*(1+tranformVector.y)
            }
            shadowCollider.SetPath(pi,setpath);
        }
        
    }

    private void imageTransform(Vector2 tranformVector, Transform lightTransform)
    {
        ShapeImage shapeImage = reflsDic[lightTransform].GetComponent<ShapeImage>();
        Sprite tmpS = Sprite.Instantiate(shapeImage.sprite);
        //Destroy(shapeImage.sprite);
        shapeImage.sprite = tmpS;
        shapeImage.offset = tranformVector.x * canvasScale * (1+tranformVector.y);
        // Debug.Log("x:"+ tranformVector.x);
        // Debug.Log("y:"+ (1+tranformVector.y));
        reflsDic[lightTransform].GetComponent<RectTransform>().sizeDelta = new Vector2(tmpS.rect.width,tmpS.rect.height*(1+tranformVector.y));
        //refDic[lightTransform].localScale = new Vector3(1,- 1 - tranformVector.y/defaultYScale,1);
    }
    
    
    
    
    
    
    public void reverseShadow(Transform lightTransform)
    {
        Transform shadowTransform = reflsDic[lightTransform];//refslDic[shadowTransform].transform;
        //TODO:Fade off Shift shadow
        Sprite lightSprite = lightTransform.gameObject.GetComponent<SpriteRenderer>().sprite;
        
        shadowTransform.GetComponent<ShapeImage>().sprite = lightSprite;
        shadowTransform.GetComponent<ShapeImage>().offset = 0;
        
        shadowTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(lightSprite.rect.width,lightSprite.rect.height);
        //TODO: Recover Collider
        createShadowCollider(lightTransform,lightSprite.rect.height/2);
        activateLightList.Remove(lightTransform);
        activateLightList.Remove(lightTransform);
        //Kill Old shadow
        //createShadow(lightTransform);
        //Fade in new Shadow
    }
    public void addShadowList(Transform shadowTransform)
    {
        activateLightList.Add(shadowTransform);
    }

    private void addNormalVertices(Transform key,PolygonCollider2D pc)
    {
        List<Vector2[]> vertices = new List<Vector2[]>();
        for (int path = 0; path < pc.pathCount; path++)
        {
            vertices.Add(pc.GetPath(path));
        }
        normalVertices.Add(key,vertices);
    }
}
