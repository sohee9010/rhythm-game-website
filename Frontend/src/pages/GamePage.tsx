import React, { useEffect, useRef } from 'react';
import { io } from 'socket.io-client';

const GamePage: React.FC = () => {
    const iframeRef = useRef<HTMLIFrameElement>(null);

    useEffect(() => {
        // SocketIO 연결
        const socket = io("http://localhost:5000");

        socket.on("connect", () => {
            console.log("Connected to Relay Server via WebSocket");
            const gameWindow = iframeRef.current?.contentWindow as any;
            if (gameWindow && gameWindow.gameInstance) {
                gameWindow.gameInstance.SendMessage("NetworkManager", "ReceiveInput", "CONNECT");
            }
        });

        socket.on("game_input", (data: any) => {
            // Unity WebGL Iframe으로 메시지 전달
            const gameWindow = iframeRef.current?.contentWindow as any;
            if (gameWindow && gameWindow.gameInstance) {
                // data.lane: "0", "1", "2", "3"
                gameWindow.gameInstance.SendMessage("NetworkManager", "ReceiveInput", data.lane.toString());
            } else {
                console.warn("Game instance not found in Iframe");
            }
        });

        return () => {
            socket.disconnect();
        };
    }, []);

    return (
        <div className="flex flex-col items-center justify-center h-screen overflow-hidden bg-black p-4">
            <h1 className="text-2xl text-white font-bold mb-2">RHYTHM MOTION</h1>
            <div className="w-full max-w-[960px] aspect-[960/600] relative">
                <iframe
                    ref={iframeRef}
                    src="/game/index.html"
                    title="Rhythm Game"
                    width="960"
                    height="600"
                    className="w-full h-full border-0 rounded-lg shadow-[0_0_50px_rgba(168,85,247,0.2)]"
                    allowFullScreen
                />
            </div>
            <p className="text-gray-500 text-sm mt-2">
                모바일 컨트롤러의 QR 코드를 스캔하여 연결하세요!
            </p>
        </div>
    );
};

export default GamePage;
