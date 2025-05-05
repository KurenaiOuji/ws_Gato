const WebSocket = require('ws');
const users = [];

class User{
	constructor()
	{
		this._username="none";
		this._conn=null;
	}

	set username( user )
	{
		this._username = user;
	}

	get username ()
	{
		return this._username;
	}

	set connection( con )
	{
		this._conn = con;
	}

	get connection ()
	{
		return this._conn;
	}

}

const wss = new WebSocket.Server({ port: 8080 },()=>{
    console.log('WebSocket - Server Started - menssage.js');
});

wss.on('connection', function connection(ws) {
	
	console.log('New connenction');
	let user = new User ();
	user.connection = ws;
	users.push(user); // Agregar la conexión (cliente) a la lista
	
    ws.on('open', (data) => {
		console.log('Now Open');
	});

	ws.on('message', (data) => {
		console.log('Data received: %s',data);
		
		//ws.send("The server response: "+data); // Para mandar el mensaje al cliente que lo envió

		let info = data.toString().split('|'); // 200|username // 100| // 300|id|pos
		let u;
		switch (info[0])
		{
			case '200': // set Username // 200|xman
				let found = false;

				users.forEach(us => {
					if(us.username === info[1])
					{
						found = true;
					}
				});

				if(!found)
				{
					user.username = info[1];
					user.connection.send("201|Cambio Exitoso!");
					
					users.forEach(us => {
						if(us.connection.readyState === WebSocket.OPEN)
						{
								us.connection.send("500|");
						}
					});
				}
				else
				{
					user.connection.send("202|Usuario Existente");
				}
				
			break; 

			case '300': // getList
				let lista = []; // array
				
				users.forEach(us => {
					if(us.connection.readyState === WebSocket.OPEN)
					{
						if( !(us.username === "none") )
							lista.push(us.username);
					}
				});

				let json = '{"users":' + JSON.stringify(lista) + '}';

				console.log("Users Send");
				user.connection.send("301|" + json);
			break;

			case '400': // Mandar mensaje directo
				u=true;

				users.forEach(us => {
					if(us.username === info[1])
					{
						u=false;
						us.connection.send("401|");
					}
				});

				if(u == true){
					user.connection.send("404|User not found");
				}

			break;

			case '401': // Mandar solicitud de juego // 401|zevs
				u=true;

				users.forEach(us => {
					if(us.username === info[1])
					{
						u=false;
						us.connection.send("400|"+info[2]);
					}
				});

				if(u == true){
					user.connection.send("404|User not found");
				}

			break;

			case '402': // Mandar solicitud de juego // 402|greys|OK // 402|grey|NO
				u=true;

				users.forEach(us => {
					if(us.username === info[1])
					{
						u=false;
						us.connection.send("400|"+info[2]);
					}
				});

				if(u == true){
					user.connection.send("404|User not found");
				}

			break;
			
				case '404': // Mandar mensaje directo
				break;

			default: // broadcast
				// Mandar a todos los clientes conectados el mensaje con el username de quien lo envió
				users.forEach(us => {
					if(us.readyState === WebSocket.OPEN)
					{
						us.send(us.username + " says: " + data); // si falla, cambiar a: `data.toString()`
					}
				});
			break;
		}
	});

	// Al cerrar la conexión, quitar de la lista de clientes
	ws.on('close', () => { 
		let index = users.indexOf(user);
		if(index > -1)
		{
			users.splice(index, 1);
			user.connection.send("UserName disconnected: "+user.username);
		}
	});
});

wss.on('listening',()=>{
   console.log('WebSocket - Now listening on port 8080 !');
});