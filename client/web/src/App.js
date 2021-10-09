import React, { useState, useEffect } from "react";
import { io } from "socket.io-client";
const ENDPOINT = "http://127.0.0.1:3000/view";

function App() {
  const [connection, setConnection] = useState("");
  const [response, setResponse] = useState("");

  useEffect(() => {
    const socket = io(ENDPOINT);
    setConnection(socket);

    socket.on("FromAPI", data => {
      const test = JSON.parse(data);
      setResponse(test);
    });

    return () => socket.disconnect();
  }, [setConnection]);

  return (
    <p>{
      `
      isInPitLane: ${response.isInPitLane}
      iCurrentTime: ${response.iCurrentTime}
      CurrentTime: ${response.CurrentTime}
      `
      }
    </p>
  );
}

export default App;
