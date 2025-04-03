const WebSocket = require('ws');

const clients = [];
const player1 = null;
const player2 = null;

const wss = new WebSocket.Server({ port: 8080 },()=>{
    console.log('Server Started');
});

wss.on('connection', function connection(ws) {
    console.log('Se conectó un cliente...');
    clients.push(ws);

    ws.on('open', (data) => {
        console.log('New Connection');
    });

ws.on('message', (data) => {
        console.log('Data received: %s',data);

        let code = data.toString().split(":");

        switch( code[0] )
        {
            case 210: // getStatus
                clients.forEach(client => {
                    if(client.readyState === WebSocket.OPEN)
                    {
                        client.send( JSON.gato );
                    }
                });

        }

        //ws.send("The server response: "+data);
    });
});

wss.on('listening',()=>{
    console.log('listening on 8080');
});

