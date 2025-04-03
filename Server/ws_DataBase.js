const WebSocket = require('ws');
const fs = require('fs');

const wss = new WebSocket.Server({ port: 8080 }, () => {
    console.log('WebSocket Server Started on port 8080');
});

class Gato {
    constructor() {
        this.db = "game.db";
        this.board = [0, 0, 0, 0, 0, 0, 0, 0, 0];
        this.p1 = "id1";
        this.p2 = "id2";
        this.actual = Math.floor(Math.random() * 2) + 1;
        this.round = 1;
        this.score1 = 0;
        this.score2 = 0;
        this.loadDb();
    }

    saveDb() {
        fs.writeFileSync(this.db, JSON.stringify(this));
    }

    loadDb() {
        if (fs.existsSync(this.db)) {
            Object.assign(this, JSON.parse(fs.readFileSync(this.db)));
        }
    }

    getStatus(id) {
        return JSON.stringify({
            actual: this.actual,
            round: this.round,
            board: this.board,
            score1: this.score1,
            score2: this.score2
        });
    }

    turn(id, pos) {
        if (this.board[pos] === 0 && this.getPlayer(id) === this.actual) {
            this.board[pos] = this.actual;
            this.changeSides();
            this.saveDb();
            return "OK";
        }
        return "Error";
    }

    getPlayer(id) {
        return id === this.p1 ? 1 : id === this.p2 ? 2 : 0;
    }

    changeSides() {
        this.actual = this.actual === 1 ? 2 : 1;
    }
}

const game = new Gato();

wss.on('connection', (ws) => {
    console.log('Nuevo cliente conectado');

    ws.on('message', (message) => {
        const [command, id, pos] = message.toString().split(":");
        let response = "";

        switch (command) {
            case "getStatus":
                response = game.getStatus(id);
                break;
            case "turn":
                response = game.turn(id, parseInt(pos));
                break;
            default:
                response = "Comando no reconocido";
        }

        ws.send(response);
    });
});

console.log('WebSocket server running on ws://localhost:8080');
