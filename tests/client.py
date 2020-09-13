import asyncio
import websockets

async def echo():
    uri = "ws://192.168.1.9"
    async with websockets.connect(uri) as websocket:
        async for message in websocket:
            print(message)
            await websocket.send(message)

asyncio.get_event_loop().run_until_complete(echo())