const WebSocket = require('ws');
const clients = [];

class Cliente{
	constructor()
	{
		this._username="No name";
		this.p1 = null;
        this.p2 = null;
        this.actual = null;
        this.round = null;
        this.score1 = null;
        this.score2 = null;
        this.board = [];
	}

	set username( user )
	{
		this._username = user;
	}

	get username ()
	{
		return this._username;
	}
}

const wss = new WebSocket.Server({ port: 8080 },()=>{
    console.log('Server Started');
});

wss.on('connection', function connection(ws) {
	
	console.log('New connenction');
	//clients.push(ws); // Agregar la conexión (cliente) a la lista

	let cliente = new Cliente ();
	
    ws.on('open', (data) => {
		console.log('Now Open');
	});

	ws.on('message', (data) => {
		console.log('Data received: %s',data);
		
		ws.send("The server response: "+data); // Para mandar el mensaje al cliente que lo envió

		let info = data.toString().split('|');

		switch (info[0])
		{
			case '200':
				cliente.username = info[1];
				ws.send("UserName upDated: "+cliente.username);
				break;

			case '201':
				console.log("iniciando Juego");
				cliente.board = [0, 0, 0, 0, 0, 0, 0, 0, 0];
        		cliente.p1 = "id1";
        		cliente.p2 = "id2";
        		cliente.actual = 1;
        		cliente.round = 1;
        		cliente.score1 = 0;
        		cliente.score2 = 0;
				break

			case '202':
				let pos = parseInt(info[1]);
				let player = parseInt(info[2]);

				if (pos >= 0 && pos < cliente.board.length) {
					cliente.board[pos - 1] = player; // Actualizar la posición en el tablero
					ws.send("Tablero actualizado: " + cliente.board.toString());
				} else {
					ws.send("Posición inválida");
				}
				break
			
				default:
					// Mandar a todos los clientes conectados el mensaje con el username de quien lo envió
					clients.forEach(client => {
						if(client.readyState === WebSocket.OPEN)
						{
							client.send(cliente.username + " says: " + data); // si falla, cambiar a: `data.toString()`
						}
					});
					break;
		}
	});

	// Al cerrar la conexión, quitar de la lista de clientes
	ws.on('close', () => { 
		let index = clients.indexOf(ws);
		if(index > -1)
		{
			clients.splice(index, 1);
			ws.send("UserName disconnected: "+cliente.username);
		}
	});
});

wss.on('listening',()=>{
   console.log('Now listening on port 8080...');
});