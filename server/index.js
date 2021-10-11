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
const UpdateFactory = require('./updateFactory');

const EVENT_NAMES = {
  REAL_TIME: 'REAL_TIME_UPDATE',  
  NEW_LAP: 'NEW_LAP_UPDATE',
  PIT_IN: 'PIT_IN_UPDATE',
  PIT_OUT: 'PIT_OUT_UPDATE',
};

const LAST_UPDATES = {
    REAL_TIME: null,
    NEW_LAP: null,
    PIT_IN: null,
    PIT_OUT: null,
};

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

    socket.on(EVENT_NAMES.REAL_TIME, (msg) => {
        console.log(`UPDATE: ${JSON.stringify(msg, null, 3)}`);
        LAST_UPDATES.REAL_TIME = msg;

        clientNameSpace.emit(EVENT_NAMES.REAL_TIME, realTimeUpdate);
    });

    socket.on(EVENT_NAMES.NEW_LAP, (msg) => {
        console.log(`NEW LAP UPDATE: ${JSON.stringify(msg, null, 3)}`);
        LAST_UPDATES.NEW_LAP = msg;

        clientNameSpace.emit(EVENT_NAMES.NEW_LAP, newLapUpdate);
    });

    socket.on(EVENT_NAMES.PIT_IN, (msg) => {
        console.log(`NEW LAP UPDATE: ${JSON.stringify(msg, null, 3)}`);
        LAST_UPDATES.PIT_IN = msg;

        clientNameSpace.emit(EVENT_NAMES.PIT_IN, newLapUpdate);
    });

    socket.on(EVENT_NAMES.PIT_OUT, (msg) => {
        console.log(`NEW LAP UPDATE: ${JSON.stringify(msg, null, 3)}`);
        LAST_UPDATES.PIT_OUT = msg;
        
        const pitStopUpdate = UpdateFactory.createPitUpdate(LAST_UPDATES.PIT_IN, LAST_UPDATES.PIT_OUT);

        clientNameSpace.emit(EVENT_NAMES.PIT_OUT, pitStopUpdate);
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
