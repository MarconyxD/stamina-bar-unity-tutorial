# English version
[Versão em Português](#Versão-em-Português)

# Creating a stamina bar for 2D games in Unity
The version of Unity used in this project was 2022.3.4f1. The project was created as a 2D project.

Several games have stamina bar mechanics for the character, so that it is consumed when actions that require effort are performed. A simple case is when we want the character to run. He may have limited breath and physical stamina, so it is interesting that he only runs for a certain amount of time. Let's produce a stamina bar that displays the character's running time.

# Moving a character
First, to simulate the character's running, it is important that he can move around the scene. So we'll start by creating a script so it can move.

To get started, let's add the following parameters:

```
public float speed = 2;
public Vector2 direction;
public bool running = false;
private Rigidbody2D playerRB;
```

The speed parameter we will use to define the character's speed, direction will be a Vector2 that will determine the direction in which the character should walk based on the buttons being pressed, running is a bool that we will use to define the moments when the character is running or not , and playerRB is the Rigidbody2D that we will use to move the character.

Now, we need to define playerRB as the Rigidbody2D component of the character that has this script as a component. This will be done at the beginning of the application, using the Start() method.

```
void Start()
{
    playerRB = GetComponent<Rigidbody2D>();
}
```

For the Update() method, we want it to be checked at all times which buttons are being pressed, so we will call a CheckInputs() method that we will define below Update().

```
void Update()
{
    CheckInputs();
}
```

So, for the CheckInputs() method, let's check the clicking of the W, A, S, D and arrow keys on the keyboard. While they are pressed, we want the direction vector to be updated with the respective direction that the key represents. We must not forget to reset this vector with each call, otherwise it will be infinitely incremented, in addition to the character not stopping moving when we release the keys.

```
void CheckInputs()
{
    direction = Vector2.zero;

    if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
    {
        direction += Vector2.left;
    }

    if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
    {
        direction += Vector2.right;
    }

    if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
    {
        direction += Vector2.up;
    }

    if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
    {
        direction += Vector2.down;
    }
}
```

At this moment, our direction vector will have movement information, but the character will not move yet, as we have not applied anything to it. To do this, we will use a FixedUpdate() method. This method, like Update(), is called continuously during the application. The difference is that, while Update() is called every frame, where there may be a time difference between calls depending on frame drops, FixedUpdate() is called at a fixed time interval. As we don't want our character to have its movement varied according to the frames, we will use FixedUpdate(). For the sake of organization, add this method between Update() and CheckInputs().

```
private void FixedUpdate()
{
    playerRB.MovePosition(playerRB.position + direction * speed * Time.deltaTime);
}
```

Here, we are making the character's Rigidbody2D move its position based on its current position and adding the direction multiplied by the speed we defined multiplied by Time.deltaTime. The latter is a time interval between frames that we use as a reference to generate consistency in the movement speed.

Thus, the final code for character movement will be as follows:

```
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 2;
    public Vector2 direction;
    public bool running = false;
    private Rigidbody2D playerRB;

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckInputs();
    }

    private void FixedUpdate()
    {
        playerRB.MovePosition(playerRB.position + direction * speed * Time.deltaTime);
    }

    void CheckInputs()
    {
        direction = Vector2.zero;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            direction += Vector2.left;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            direction += Vector2.right;
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            direction += Vector2.up;
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            direction += Vector2.down;
        }
    }
}
```

Don't forget to save your script after making all changes, otherwise the new lines of code will not be executed in Unity!

Now, in Unity, we need to perform some configurations. The first of them is to create our character. Create a GameObject and name it whatever you want. Add a SpriteRenderer so you can view it on screen. Add any Sprite to it. In my case, I will just add a black rectangle. Now add a Rigidbody2D. By adding Rigidbody2D, the GameObject will be affected by gravity when we press Play. So that it doesn't fall to infinity and we can perform the tests, change the Gravity Scale value to zero. Finally, add the script that was created to the character.

https://github.com/MarconyxD/stamina-bar-unity-tutorial/assets/71736128/2990f995-c07c-4caa-bcf1-526f99a45335

# Creating the stamina bar
For the stamina bar we will need two images: one to represent the bar that will decrease and the other to represent the background of the bar. Add two Images to the scene by going to UI -> Image. You can name them whatever you want.

Resize and move the images in the scene however you want. Just make sure that one is on top of the other, with the same size and different colors, so that you can tell them apart. To make the image work as a progress bar, you need to add a sprite to it. After adding a Sprite to the Image in front, you will notice that some new options will appear in the Inspector, such as Image Type. Change the Image Type from Simple to Filled. This will make it possible to use it as a progress bar. Change Fill Method to Horizontal and keep Fill Origin as Left. When changing the Fill Amount value, you will notice that the bar will decrease and increase as this value changes. The Background Image is exposed as we decrease the bar. It is through this value that we will control the filling of the bar. Change the fill shape settings if desired.

Now, we will need to create a new script. Create this script and call it whatever you want, I'll call mine StaminaControl.

Let's start by adding some parameters that we will use. First, an Image type parameter called staminaUI, which will be our image in the scene. Note that for this Image type it is necessary to add using UnityEngine.UI; at the beginning of the code, as we will need this import for it. The staminaDuration parameter determines the maximum time in seconds that the stamina bar will last. I set it to 5 seconds, but you can change this value if you wish. A currentStamina parameter to save the stamina value at each moment. We will also need to reference the playable character, so we will need a GameObject player for the character. Finally, a bool to identify the moments in which the player can run, because when the stamina is empty, it will not be possible.

```
public Image staminaUI;
public float staminaDuration = 5f;
public float currentStamina;
public GameObject player;
private bool canRun = true; 
```

Once the application starts, we will set the stamina bar to its maximum value. It is also important to make currentStamina receive the maximum value at the beginning.

```
void Start()
{
    staminaUI.fillAmount = 1f;
    currentStamina = staminaDuration;
}
```

Then, in Update(), we will perform all the necessary actions and checks for the bar to work.

First, to make the stamina go down, we need to check if the character is running, so we will check the value of the bool running parameter that exists in the character's Movement script. We also need to check if the character can run, so we will check the value of the canRun parameter. Finally, we need to check that the character, even if pressing the button to run, is not standing still, as we do not want him to spend stamina standing still, so we will check if the direction parameter in the character's Movement script is equal to zero. If all conditions are met, we will decay stamina using Time.deltaTime. We use it a lot to count the past time, since it has the value of the time interval between frames. We then update the stamina bar with a percentage value. As we know, the Fill Amount value is only between 0 and 1, therefore, we need to calculate the current percentage of stamina to establish it at a value between 0 and 1. To do this, we divide the current value of the stamina time by the total time . Finally, we update the value of the character's speed parameter to a higher value, since he is running. When walking, the value was 2, now, when running, the value becomes 6. You can use other values, if you wish.

```
if (player.GetComponent<Movement>().running && canRun && player.GetComponent<Movement>().direction != Vector2.zero)
{
    currentStamina -= Time.deltaTime;
    staminaUI.fillAmount = currentStamina / staminaDuration;
    player.GetComponent<Movement>().speed = 6;
}
```

The next step is to add an else if to check if the player has stopped pressing the run button. As this method is Update(), it is called all the time, therefore, we always need to check the current status of each parameter and update the current state of the game. So, let's check if the character's running parameter is no longer true and if the currentStamina value is less than the total stamina value, that is, less than staminaDuration. If so, we want stamina to be recovered, but at a slower speed than when it drops. In this case, I am dividing this increase value by 5, making stamina recovery 5 times slower. If you want it to be faster, decrease this value, and if you want it to be slower, increase it. Then, as we did before, we update the stamina fill value using a value between 0 and 1 and then update the character's speed value back to 2.

```
else if (!player.GetComponent<Movement>().running && currentStamina <= staminaDuration)
{
    currentStamina += Time.deltaTime / 5;
    staminaUI.fillAmount = currentStamina / staminaDuration;
    player.GetComponent<Movement>().speed = 2;
}
```

Another situation that can happen is the player using up all their stamina instead of stopping spending it before it runs out. We also need to consider this situation. So, if the current stamina value is equal to or less than zero, the player will no longer be able to run, changing the canRun bool to false, and their speed will return to the initial value, which was 2.

```
if (currentStamina <= 0f)
{
    canRun = false;
    player.GetComponent<Movement>().speed = 2;
}
```

But any time his current stamina is greater than zero, it is possible for him to run.

```
else if (currentStamina > 0f)
{
    canRun = true;
}
```

So, we have finished the script for the stamina bar. The full script can be viewed below:

```
using UnityEngine;
using UnityEngine.UI;

public class StaminaControl : MonoBehaviour
{
    public Image staminaUI;
    public float staminaDuration = 5f;
    public float currentStamina;
    private GameObject player;
    private bool canRun = true;

    void Start()
    {
        staminaUI.fillAmount = 1f;
        currentStamina = staminaDuration;
    }

    void Update()
    {
        if (player.GetComponent<Movement>().running && canRun && player.GetComponent<Movement>().direction != Vector2.zero)
        {
            currentStamina -= Time.deltaTime; 
            staminaUI.fillAmount = currentStamina / staminaDuration;
            player.GetComponent<Movement>().speed = 6;
        }
        else if (!player.GetComponent<Movement>().running && currentStamina <= staminaDuration)
        {
            currentStamina += Time.deltaTime / 5;
            staminaUI.fillAmount = currentStamina / staminaDuration;
            player.GetComponent<Movement>().speed = 2;
        }

        if (currentStamina <= 0f)
        {
            canRun = false;
            player.GetComponent<Movement>().speed = 2;
        }
        else if (currentStamina > 0f)
        {
            canRun = true;
        }
    }
}
```

Don't forget to save your script after making all changes, otherwise the new lines of code will not be executed in Unity!

Now, as usual, we need to make some configurations in Unity.

First, let's create a new GameObject. This GameObject will be responsible for controlling stamina, so I will name it StaminaControl as well. We added the StaminaControl script to it and, in its Inspector, we added the Image that we defined as the stamina bar to the staminaUI parameter and the character's GameObject to the player parameter.

Now, we need to go back to the character's script and define which button we will use to be responsible for making the character run.

In the CheckInputs() method add the following lines of code:

```
if (Input.GetKey(KeyCode.Mouse1) && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow)
            || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.DownArrow)
            || Input.GetKey(KeyCode.S)))
{
    running = true;
}

if (Input.GetKeyUp(KeyCode.Mouse1))
{
    running = false;
}
```

In my case, I defined the right mouse button as being responsible for the running action. If you want to use another one, simply change Mouse1 in the lines of code to the desired button. Thus, if the right mouse button and any of the movement buttons are being pressed, the value of the bool running parameter is changed to true. The moment the player releases the right mouse button, the running parameter becomes false.

Don't forget to save your script after making all changes, otherwise the new lines of code will not be executed in Unity!

Now, we can test and verify the result.

https://github.com/MarconyxD/stamina-bar-unity-tutorial/assets/71736128/75d18a5b-163e-4ba1-bb0e-0b5671c9faaf

# Versão em Português
[English version](#English-version)

# Criando uma barra de estamina para jogos 2D na Unity
A versão da Unity utilizada neste projeto foi a 2022.3.4f1. O projeto foi criado como sendo um projeto 2D.

Diversos jogos possuem mecânica de barra de estamina para o personagem, de forma que a mesma é consumida quando ações que demandam esforço são realizadas. Um caso simples é quando queremos que a personagem corra. O mesmo pode ter o fôlego e disposição física limitados, portanto é interessante que ele corra apenas por um determinado tempo. Vamos produzir uma barra de estamina que expõe esse tempo de corrida da personagem.

# Movendo um personagem
Primeiro, para simular a corrida do personagem, é importante que ele consiga se mover no cenário. Então, começaremos criando um script para que ele possa se mover.

Para começar, vamos adicionar os seguintes parâmetros:

```
public float speed = 2;
public Vector2 direction;
public bool running = false;
private Rigidbody2D playerRB;
```

O parâmetro speed iremos utilizar para definir a velocidade do personagem, direction será um Vector2 que determinará a direção a qual o personagem deverá andar com base nos botões sendo apertados, running é um bool que utilizaremos para definir os momentos que o personagem está correndo ou não, e playerRB é o Rigidbody2D que utilizaremos para efetuar a movimentação da personagem.

Agora, precisamos definir playerRB como sendo o Rigidbody2D componente do próprio personagem que possuir este script como componente. Isto será feito logo ao início da aplicação, por meio do método Start().

```
void Start()
{
    playerRB = GetComponent<Rigidbody2D>();
}
```

Para o método Update(), queremos que seja verificado a todo momento quais botões estão sendo apertados, então iremos chamar um método CheckInputs() que iremos definir abaixo do Update().

```
void Update()
{
    CheckInputs();
}
```

Então, para o método CheckInputs(), vamos conferir o clicar das teclas W, A, S, D e das setas do teclado. Enquanto elas estiverem pressionadas, queremos que o vetor direction seja atualizado com a respectiva direção que a tecla representa. Não podemos esquecer de zerar este vetor a cada chamada, senão ele será incrementado infinitamente, além do personagem não parar de se mover quando soltarmos as teclas.

```
void CheckInputs()
{
    direction = Vector2.zero;

    if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
    {
        direction += Vector2.left;
    }

    if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
    {
        direction += Vector2.right;
    }

    if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
    {
        direction += Vector2.up;
    }

    if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
    {
        direction += Vector2.down;
    }
}
```

Neste momento, nosso vetor direction irá possuir as informações de movimento, mas o personagem ainda não irá se mover, pois não aplicamos nada a ele. Para isto, vamos utilizar um método FixedUpdate(). Este método, assim como o Update(), é chamado continuamente durante a aplicação. A diferença é que, enquanto o Update() é chamado a cada frame, onde pode-se haver diferença de tempo entre as chamadas a depender das quedas de frames, o FixedUpdate() é chamado em um intervalo fixo de tempo. Como não queremos que nosso personagem tenha seu movimento variado conforme os frames, utilizaremos o FixedUpdate(). Por questão de organização, adicione este método entre o Update() e o CheckInputs().

```
private void FixedUpdate()
{
    playerRB.MovePosition(playerRB.position + direction * speed * Time.deltaTime);
}
```

Aqui, estamos fazendo com que o Rigidbody2D do personagem realize a movimentação da posição dele com base na posição atual e adicionando a direção multiplicada pela velocidade que definimos multiplicada por Time.deltaTime. Este último é um intervalo de tempo entre os frames que utilizamos de referência para gerar consistência à velocidade de movimentação.

Assim, o código final para movimentação do personagem será o seguinte:

```
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 2;
    public Vector2 direction;
    public bool running = false;
    private Rigidbody2D playerRB;

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckInputs();
    }

    private void FixedUpdate()
    {
        playerRB.MovePosition(playerRB.position + direction * speed * Time.deltaTime);
    }

    void CheckInputs()
    {
        direction = Vector2.zero;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            direction += Vector2.left;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            direction += Vector2.right;
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            direction += Vector2.up;
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            direction += Vector2.down;
        }
    }
}
```

Não se esqueça de salvar seu script após realizar todas as alterações, senão as novas linhas de código não serão executadas na Unity!

Agora, na Unity, precisamos realizar algumas configurações. A primeira delas é criar nosso personagem. Crie um GameObject e nomeie-o da forma que desejar. Adicione um SpriteRenderer para que seja possível visualizá-lo na tela. Adicione qualquer Sprite a ele. No meu caso, irei apenas adicionar um retângulo de cor preta. Agora, adicione um Rigidbody2D. Ao adicionar o Rigidbody2D, o GameObject passará a sofrer efeito da gravidade quando dermos Play. Para que ele não caia para o infinito e consigamos realizar os testes, altere o valor de Gravity Scale para zero. Por fim, adicione ao personagem o script que foi criado.

https://github.com/MarconyxD/stamina-bar-unity-tutorial/assets/71736128/2990f995-c07c-4caa-bcf1-526f99a45335

# Criando a barra de stamina
Para a barra de stamina iremos precisar de duas imagens: uma para representar a barra que irá diminuir e a outra para representar o plano de fundo da barra. Adicione duas Images à cena indo em UI -> Image. Pode nomeá-las da forma que desejar.

Redimensione e mova as imagens na cena da forma que desejar. Apenas tenha certeza de que uma estará sobre a outra, com o mesmo tamanho e com cores diferentes, para que seja possível diferenciá-las. Para que seja possível fazer com que a imagem funcione como uma barra de progresso é necessário adicionar um Sprite a ela. Após adicionar um Sprite à Image que está na frente, irá observar que algumas novas opções irão surgir no Inspector, como a Image Type. Troque o Image Type de Simple para Filled. Isso fará com que seja possível utilizá-la como uma barra de progresso. Altere Fill Method para Horizontal e mantenha Fill Origin como Left. Ao alterar o valor de Fill Amount, irá perceber que a barra irá diminuir e aumentar, conforme alterar este valor. A Image de fundo é exposta conforme diminuímos a barra. É por meio deste valor que iremos controlar o preenchimento da barra. Altere as configurações da forma de preenchimento, caso desejar.

Agora, iremos precisar criar um novo script. Crie este script e chame-o da forma que desejar, Irei chamar o meu de StaminaControl.

Vamos começar adicionando alguns parâmetros que iremos utilizar. Primeiro, um parâmetro do tipo Image chamado staminaUI, que será a nossa imagem na cena. Note que para este tipo Image é necessário adicionar using UnityEngine.UI; no início do código, pois para ele iremos precisar desta importação. O parâmetro staminaDuration determina o tempo máximo em segundos que a barra de stamina irá durar. Eu defini como 5 segundos, mas pode alterar este valor, se desejar. Um parâmetro currentStamina para salvar o valor da stamina a cada momento. Também iremos precisar referenciar o personagem jogável, por isso iremos precisar de um GameObject player para o personagem. Por fim, um bool para identificar os momentos que o jogador poderá correr, pois, quando a stamina estiver vazia, não será possível.

```
public Image staminaUI;
public float staminaDuration = 5f;
public float currentStamina;
public GameObject player;
private bool canRun = true; 
```

Assim que a aplicação iniciar, vamos definir a barra de stamina com seu valor máximo. Também é importante fazer com que a currentStamina receba o valor máximo no início.

```
void Start()
{
    staminaUI.fillAmount = 1f;
    currentStamina = staminaDuration;
}
```

Em seguida, em Update(), iremos realizar todas as ações e verificações necessárias para que a barra funcione.

Primeiro, para fazer com que a stamina desça, precisamos verificar se o personagem está correndo, por isso iremos verificar o valor do parâmetro bool running que existe no script Movement do personagem. Também precisamos verificar se o personagem pode correr, então iremos verificar o valor do parâmetro canRun. Por fim, precisamos verificar se o personagem, mesmo que apertando o botão para correr, não está parado, pois não queremos que ele gaste stamina parado, então iremos verificar se o parâmetro direction no script Movement do personagem é igual a zero. Se todas as condições forem cumpridas, iremos realizar o decaimento da stamina pelo Time.deltaTime. Utilizamos muito ele para contar o tempo passado, uma vez que ele possui o valor de intervalo de tempo entre os frames. Em seguida, atualizamos a barra de stamina com um valor de porcentagem. Como sabemos, o valor de Fill Amount fica apenas entre 0 e 1, portanto, precisamos calcular a porcentagem atual da stamina para estabelece-la em um valor entre 0 e 1. Para isto, dividimos o valor atual do tempo de stamina pelo tempo total. Por fim, atualizamos o valor do parâmetro speed do personagem para um valor maior, uma vez que ele está correndo. Andando o valor era 2, agora, correndo, o valor passa a ser 6. Pode utilizar outros valores, se desejar.

```
if (player.GetComponent<Movement>().running && canRun && player.GetComponent<Movement>().direction != Vector2.zero)
{
    currentStamina -= Time.deltaTime;
    staminaUI.fillAmount = currentStamina / staminaDuration;
    player.GetComponent<Movement>().speed = 6;
}
```

O próximo passo é adicionar um else if para verificar se o jogador parou de apertar o botão de correr. Como este método é o Update(), ele é chamado a todo momento, portanto, precisamos sempre verificar a atual situação de cada parâmetro e atualizar o estado atual do jogo. Então, vamos verificar se o parâmetro running do personagem não é mais verdadeiro e se o valor de currentStamina é menor que o valor da stamina total, ou seja, menor que staminaDuration. Em caso positivo, queremos que a stamina seja recuperada, porém em velocidade mais lenta do que quando ela desce. Neste caso, eu estou dividindo este valor de acréscimo por 5, tornando 5 vezes mais lenta a recuperação de stamina. Se quiser que seja mais rápido, diminua este valor, e se quiser que seja mais lento, aumente ele. Em seguida, como fizemos anteriormente, atualizamos o valor do preenchimento da stamina utilizando um valor entre 0 e 1 e depois atualizamos o valor de velocidade do personagem de volta para 2.

```
else if (!player.GetComponent<Movement>().running && currentStamina <= staminaDuration)
{
    currentStamina += Time.deltaTime / 5;
    staminaUI.fillAmount = currentStamina / staminaDuration;
    player.GetComponent<Movement>().speed = 2;
}
```

Outra situação que pode acontecer é o jogador gastar toda a stamina ao invés de parar de gastá-la antes de acabar. Precisamos também considerar esta situação. Então, se o valor de stamina atual for igual ou menor que zero, o jogador não poderá mais correr, alterando o bool canRun para false, além de que sua velocidade voltará para o valor inicial, que era 2.

```
if (currentStamina <= 0f)
{
    canRun = false;
    player.GetComponent<Movement>().speed = 2;
}
```

Mas a qualquer momento que a stamina atual for maior que zero, é possível para ele correr.

```
else if (currentStamina > 0f)
{
    canRun = true;
}
```

Assim, finalizamos o script para a barra de stamina. O script completo pode ser visualizado abaixo:

```
using UnityEngine;
using UnityEngine.UI;

public class StaminaControl : MonoBehaviour
{
    public Image staminaUI;
    public float staminaDuration = 5f;
    public float currentStamina;
    private GameObject player;
    private bool canRun = true;

    void Start()
    {
        staminaUI.fillAmount = 1f;
        currentStamina = staminaDuration;
    }

    void Update()
    {
        if (player.GetComponent<Movement>().running && canRun && player.GetComponent<Movement>().direction != Vector2.zero)
        {
            currentStamina -= Time.deltaTime; 
            staminaUI.fillAmount = currentStamina / staminaDuration;
            player.GetComponent<Movement>().speed = 6;
        }
        else if (!player.GetComponent<Movement>().running && currentStamina <= staminaDuration)
        {
            currentStamina += Time.deltaTime / 5;
            staminaUI.fillAmount = currentStamina / staminaDuration;
            player.GetComponent<Movement>().speed = 2;
        }

        if (currentStamina <= 0f)
        {
            canRun = false;
            player.GetComponent<Movement>().speed = 2;
        }
        else if (currentStamina > 0f)
        {
            canRun = true;
        }
    }
}
```

Não se esqueça de salvar seu script após realizar todas as alterações, senão as novas linhas de código não serão executadas na Unity!

Agora, como de praxe, precisamos realizar algumas configurações na Unity. 

Primeiro, vamos criar um novo GameObject. Este GameObject será responsável pelo controle da stamina, portanto, irei nomeá-lo como StaminaControl também. Adicionamos o script StaminaControl a ele e, no Inspector dele, adicionamos a Image que definimos como a barra de stamina ao parâmetro staminaUI e o GameObject do personagem ao parâmetro player.

Agora, precisamos voltar ao script do personagem e definir qual botão utilizaremos para ser o responsável para fazer com que o personagem corra.

No método CheckInputs() adicione as seguintes linhas de código:

```
if (Input.GetKey(KeyCode.Mouse1) && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow)
            || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.DownArrow)
            || Input.GetKey(KeyCode.S)))
{
    running = true;
}

if (Input.GetKeyUp(KeyCode.Mouse1))
{
    running = false;
}
```

No meu caso, defini o botão direito do mouse como sendo o responsável pela ação de corrida. Se desejar utilizar outro, basta alterar o Mouse1 nas linhas de código pelo botão desejado. Assim, se o botão direito do mouse e algum dos botões de movimento estiverem sendo pressionados, o valor do parâmetro bool running é alterado para verdadeiro. No momento que o jogador solta o botão direito do mouse, o parâmetro running passa a ser false.

Não se esqueça de salvar seu script após realizar todas as alterações, senão as novas linhas de código não serão executadas na Unity!

Agora, podemos testar e verificar o resultado.

https://github.com/MarconyxD/stamina-bar-unity-tutorial/assets/71736128/75d18a5b-163e-4ba1-bb0e-0b5671c9faaf
