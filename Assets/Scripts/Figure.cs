using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Figure : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int _id;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private ShapeType _shapeType;
    [SerializeField] private ColorType _frameColor;
    [SerializeField] private AnimalType _animalType;

     private Rigidbody2D _rigidbody;
     private Collider2D _collider2D;
    private bool _interactable = true;


    public int ID => _id;
    public GameObject Prefab => _prefab;  
    public ShapeType Shape => _shapeType;    
    public ColorType FrameColor => _frameColor; 
    public AnimalType Animal => _animalType;

    public void SetId(int value) => _id = value;

    public void Initialize()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        EventManager.OnGameLose.AddListener(MakeObjectNotInteractable);
        Debug.Log("Init");

    }

    private void MakeObjectNotInteractable()
    {
        _interactable = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_interactable) return;
        Select();
    }

    public void Select()
    {
        _interactable = false;
        _rigidbody.bodyType = RigidbodyType2D.Static;
        _collider2D.enabled = false;
        EventManager.SetObjectSelected(this);
    }
}

public enum ShapeType { Circle, Square, Triangle }
public enum ColorType { Green, Blue, Pink }
public enum AnimalType { Pig, Dog, Bear }
