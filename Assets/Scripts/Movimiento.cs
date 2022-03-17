using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movimiento : MonoBehaviour
{
    [Header("Parametros Editables")]
    [SerializeField] private float Velocidad = 1.5f;
    [SerializeField] private float FuerzaSalto = 5f;
    [SerializeField] private LayerMask sueloLayer=0;
    [SerializeField] public Transform poloTierra;
    [SerializeField] public Text textoMonedas , textoManzanas , textoHuevos;
    
    #region Llamado_parametros
    //private int _CorrerAnimatorParameter = Animator.StringToHash("correr");
    private int _CaminaAnimatorParameter = Animator.StringToHash("caminar");

    //private int _IdleAnimatorParameter = Animator.StringToHash("Idle");
    #endregion
    
    
    #region Componentes_Objetos
        protected Rigidbody2D _rigidbody;
        protected Animator _animator;
       
    #endregion

    #region Variables
        public bool estaEnTierra;
        private Vector2 _move;
        float h;//Axis Horizontal
        bool r;//Correr
        bool a;//Acción
        private float hVel;//Velocidad en la Horizontal
        private bool flip = false;//Girar sprite personaje
        private float radius = 0.03f; 
        float v;//Axis Vertical
        
        private int coins;
        private int apples;
        private int eggs;
        public bool BorrarPrefs;
    #endregion
    
    void Start(){
       _rigidbody = GetComponent<Rigidbody2D>();
       _animator = GetComponent<Animator>();  
       coins = PlayerPrefs.GetInt("Monedas",0); 
       apples = PlayerPrefs.GetInt("Manzanas",0);
       eggs = PlayerPrefs.GetInt("Huevos",0); 
        textoMonedas.text = coins+"";
        textoManzanas.text = apples+"";
        textoHuevos.text= eggs+"";
    }  
    private void ResetValores(){
        PlayerPrefs.SetInt("Monedas",0); 
        PlayerPrefs.SetInt("Manzanas",0);
        PlayerPrefs.SetInt("Huevos",0); 
        textoMonedas.text = "0";
        textoManzanas.text = "0";
        textoHuevos.text= "0";
    } 
    void Update(){       
        LocalInput();
        SetAnimation();
        if(BorrarPrefs){
            ResetValores();
            BorrarPrefs=false;
        }
    }
    void FixedUpdate(){
        horizontalMove(); 
        verticalMove();       
    }

    #region Metodos
    protected void LocalInput(){
           
        h = Input.GetAxisRaw("Horizontal");
        r = Input.GetButton("Jump");
        a = Input.GetButton("Action");
        //v = Input.GetAxisRaw("Vertical");
    
    }
    protected void horizontalMove(){
        _move = new Vector2(h, v);
        if(permitirMovimiento()){//Si el movimiento es permitido se desplazará sobre X
        
            hVel = _move.normalized.x * Velocidad; 
             _animator.SetBool("caminar",true);           
            _rigidbody.velocity = new Vector2(hVel, _rigidbody.velocity.y);
        }
        else{
            _animator.SetBool("caminar",false); 
        }
        GirarSprite();
    }
   protected void verticalMove(){
       if(permitirMovimiento() && r){           
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, FuerzaSalto); 
       }           
    }
    public bool permitirMovimiento(){
        estaEnTierra = Physics2D.OverlapCircle(poloTierra.position, radius, sueloLayer);
        if(estaEnTierra)
            return true;
        
        return false; 
    }

    private void GirarSprite(){
        //Se inicializa flip en false y el personaje inicia mirando hacia la derecha
                
        if ((h < 0)&&(flip == false)){            
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
            flip = true;
        }
        if ((h > 0)&& (flip == true)){            
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
            flip = false;
        }
    }
    #endregion
   
    public void SetAnimation(){       
        if(permitirMovimiento()){
          _animator.SetBool(_CaminaAnimatorParameter, h != 0.0f && estaEnTierra );        
        }      
    }
    private void capturaMonedaCollision(Collision2D objeto){
        objeto.gameObject.SetActive(false);
        coins++;
        PlayerPrefs.SetInt("Monedas",coins);
        textoMonedas.text=coins+"";
    }
    private void capturaMoneda(Collider2D objeto){
        objeto.gameObject.SetActive(false);
        coins++;
        PlayerPrefs.SetInt("Monedas",coins);
        textoMonedas.text=coins+"";
    }
    private void capturaManzana(Collider2D objeto){
        objeto.gameObject.SetActive(false);
        apples++;
        PlayerPrefs.SetInt("Manzanas",apples);
        textoManzanas.text= apples+"";
    }
    private void capturaHuevo(Collider2D objeto){
        objeto.gameObject.SetActive(false);
        eggs++;
        PlayerPrefs.SetInt("Huevos",eggs);
        textoHuevos.text=eggs+"";
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name + " : Alcanzado");
        if(col.gameObject.layer == LayerMask.NameToLayer("Moneda" ))
            capturaMoneda(col);
        if(col.gameObject.layer == LayerMask.NameToLayer("Manzana" ))
            capturaManzana(col);  

        if(col.gameObject.layer == LayerMask.NameToLayer("Huevo" ) && a)
            capturaHuevo(col);           
        
    }

    void OnCollisionEnter2D(Collision2D collision){
        
        if(collision.gameObject.layer == LayerMask.NameToLayer("Moneda" )){
            Debug.Log(collision.gameObject.name + " : Alcanzado ");
            capturaMonedaCollision(collision);
        }
    }
}
