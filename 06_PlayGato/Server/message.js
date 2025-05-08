const WebSocket = require('ws');
const users = [];
const chatRooms = {}; // clave: "usuario1_usuario2", valor: [mensajes]

class User {
	constructor() {
		this._username = "none";
		this._conn = null;
		this._status = "available";
	}

	set username(user) { this._username = user; }
	get username() { return this._username; }

	set connection(con) { this._conn = con; }
	get connection() { return this._conn; }

	set status(stat) { this._status = stat; }
	get status() { return this._status; }
}

function getRoomKey(user1, user2) {
	return [user1, user2].sort().join('_');
}

const wss = new WebSocket.Server({ port: 8080 }, () => {
	console.log('WebSocket - Server Started - menssage.js');
});

wss.on('connection', function connection(ws) {
	console.log('New connection');
	let user = new User();
	user.connection = ws;
	users.push(user);

	ws.on('open', (data) => {
		console.log('Now Open');
	});

	ws.on('message', (data) => {
		console.log('Data received: %s', data);
		let info = data.toString().split('|');
		let u;

		switch (info[0]) {
			case '200': // Set username
				let found = false;
				users.forEach(us => {
					if (us.username === info[1]) {
						found = true;
					}
				});

				if (!found) {
					user.username = info[1];
					user.connection.send("201|Cambio Exitoso!");

					users.forEach(us => {
						if (us.connection.readyState === WebSocket.OPEN) {
							us.connection.send("500|");
						}
					});
				} else {
					user.connection.send("202|Usuario Existente");
				}
				break;

			case '300': // Get user list
				let lista = [];
				users.forEach(us => {
					if (us.connection.readyState === WebSocket.OPEN && us.status === "available" && us.username !== "none" && us.username !== user.username) {
						lista.push(us.username);
					}
				});
				let json = '{"users":' + JSON.stringify(lista) + '}';
				console.log("Users Send");
				user.connection.send("301|" + json);
				break;

			case '400': // Responder a solicitud de juego
				u = true;
				users.forEach(us => {
					if (us.username === info[1]) {
						u = false;
						us.connection.send("401|");
					}
				});
				if (u == true) {
					user.connection.send("404|User not found");
				}
				break;

			case '401': // Enviar solicitud de juego
				u = true;
				users.forEach(us => {
					if (us.username === info[1]) {
						u = false;
						us.connection.send("400|" + info[2]);
					}
				});
				if (u == true) {
					user.connection.send("404|User not found");
				}
				break;

			case '402': // Respuesta a solicitud de juego
				if (info[2] === "OK") {
					let player1 = user;
					let player2 = users.find(us => us.username === info[1]);

					if (player2) {
						player1.status = "busy";
						player2.status = "busy";

						player1.connection.send("403|Iniciando partida con " + player2.username);
						player2.connection.send("403|Iniciando partida con " + player1.username);
					}
				} else {
					let player2 = users.find(us => us.username === info[1]);
					if (player2) {
						player2.connection.send("400| No");
					}
				}
				break;

			case '600': // Enviar mensaje privado con historial
				const destinatario = info[1];
				const mensaje = info.slice(2).join('|');
				const ahora = new Date();
				const hora = ahora.getHours().toString().padStart(2, '0');
				const minutos = ahora.getMinutes().toString().padStart(2, '0');
				const tiempo = `${hora}.${minutos}`;
				const mensajeFormateado = `${tiempo} ${user.username}: ${mensaje}`;
				const roomKey = getRoomKey(user.username, destinatario);

				if (!chatRooms[roomKey]) {
					chatRooms[roomKey] = [];
				}
				chatRooms[roomKey].push(mensajeFormateado);

				const destinatarioUsuario = users.find(us => us.username === destinatario);
				if (destinatarioUsuario && destinatarioUsuario.connection.readyState === WebSocket.OPEN) {
					destinatarioUsuario.connection.send(`600|${mensajeFormateado}`);
				}
				user.connection.send(`600|${mensajeFormateado}`);
				break;

			case '601': // Obtener historial
				const otroUsuario = info[1];
				const claveSala = getRoomKey(user.username, otroUsuario);
				const historial = chatRooms[claveSala] || [];
				user.connection.send(`601|${JSON.stringify(historial)}`);
				break;

			default: // Broadcast
				users.forEach(us => {
					if (us.connection.readyState === WebSocket.OPEN) {
						us.connection.send(user.username + " says: " + data);
					}
				});
				break;
		}
	});

	ws.on('close', () => {
		let index = users.indexOf(user);
		if (index > -1) {
			users.splice(index, 1);
			console.log(`User ${user.username} disconnected`);
		}
	});
});

wss.on('listening', () => {
	console.log('WebSocket - Now listening on port 8080!');
});
