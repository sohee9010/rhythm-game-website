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
            <h1 className="text-2xl text-white font-bold mb-2">Rhythm School</h1>
            <div className="w-full max-w-[1280px] aspect-video relative group shadow-2xl">
                <button
                    onClick={() => {
                        console.log("Close button clicked");
                        const gameWindow = iframeRef.current?.contentWindow as any;
                        if (gameWindow) {
                            console.log("Game window found");
                            if (gameWindow.gameInstance) {
                                console.log("Sending CANCEL message to Unity");
                                gameWindow.gameInstance.SendMessage("NetworkManager", "ReceiveInput", "CANCEL");
                            } else {
                                console.warn("gameInstance not found in gameWindow");
                            }
                        } else {
                            console.warn("Iframe contentWindow not found");
                        }
                    }}
                    className="absolute top-4 right-4 z-20 bg-red-600/80 hover:bg-red-600 text-white p-2 rounded-full transition-all duration-300 opacity-0 group-hover:opacity-100 backdrop-blur-sm"
                    title="Close QR Code"
                >
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                    </svg>
                </button>
                <iframe
                    ref={iframeRef}
                    src={`${import.meta.env.BASE_URL}game/index.html?v=${Date.now()}`}
                    title="Rhythm Game"
                    width="100%"
                    height="100%"
                    className="w-full h-full border-0 rounded-lg shadow-[0_0_50px_rgba(168,85,247,0.2)] bg-black"
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
