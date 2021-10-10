const express = require('express');
const cors = require('cors');
const app = express();
app.use(cors());
const http = require('http');
const server = http.createServer(app);
const { Server } = require("socket.io");
const io = new Server(server, {
    cors: {
        origin: '*',
        methods: ['GET', 'POST']
    }
});

var realTimeUpdate = { };
var newLapUpdate = { };

const dataSourceNameSpace = io.of("/data");
const clientNameSpace = io.of('/view');

app.get('/', (req, res) => {  
    res.send('<h1>Hello world</h1>');
});

server.listen(3000, () => {  
    console.log('listening on *:3000');
});

// from desktop client to server
dataSourceNameSpace.on('connection', (socket) => {  
    console.log('a data source connected');

    
    socket.on('real-time-update', (msg) => {
        console.log(`UPDATE: ${JSON.stringify(msg, null, 3)}`);
        realTimeUpdate = msg;
        clientNameSpace.emit('real-time-update', realTimeUpdate);
    });

    socket.on('new-lap-update', (msg) => {
        console.log(`NEW LAP UPDATE: ${JSON.stringify(msg, null, 3)}`);
        newLapUpdate = msg;
        clientNameSpace.emit('new-lap-update', newLapUpdate);
    });

    socket.on('disconnect', () => {
        console.log('a data source disconnected');
    });
});

// from server to web client
clientNameSpace.on('connection', (socket) => {
    console.log('a web interface connected');

    socket.on('disconnect', () => {
        console.log('a web interface disconnected');
    });
});
