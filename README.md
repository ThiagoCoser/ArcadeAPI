# Interface para carregar jogos em um menu de seleção ARCADE

Este projeto foi desenvolvido para utilizar os jogos dos alunos do curso de Design Digital da Facamp em um arcade, mostrando assim os projetos dos alunos para o mundo :)

# O que faz?
O programa procura arquivos .exe dentro de cada uma das pastas contidas na pasta "Jogos", exibindo um arquivo de texto de informações e uma imagem de cada projeto, em um menu de seleção. Ao selecionar o jogo, ele fica em segundo plano, possibilitando sairmos do jogo e voltar ao menu de seleção

# Como usar

- Faça uma build do projeto ou baixe o release V1.0
- Crie uma pasta chamada "Jogos" na raiz da build
- Coloque cada jogo em uma pasta separada dentro da pasta "Jogos"
- Coloque um banner com o nome "icon.png" com 1920 x 1080 pixels, assim como um arquivo de informações do projeto "info.txt" em cada pasta de jogo

# Comandos

- SETAS - Seleciona o jogo
- ENTER - Entra no jogo selecionado
- ESC - Sai do jogo selcionado / Sai do aplicativo


# V1.1
- Para projetos feitos em Godot, adicionar o script no projeto:

extends Node

```c

# Método chamado a cada quadro
func _process(delta):

	# Verifica se a tecla ESC foi pressionada

	if Input.is_action_pressed("ui_cancel"):

		# Método para sair do jogo
		get_tree().quit()
```
