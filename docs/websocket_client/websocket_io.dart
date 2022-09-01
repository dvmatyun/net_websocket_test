import 'dart:async';
import 'dart:io' as io;
import 'dart:convert' show utf8;

//ws://dvmatyun.site/ws/websocket/990d6953-774e-4ccd-98e0-73e7076d402c
//ws://127.0.0.1:42627/websocket/guid
//ws:///localhost:5000/ws
const String wsLiteral = 'ws'; // ws
const String wsaddress  = 'dvmatyun.site';//'localhost';'dvmatyun.site'
const String wspath     = '/ws/websocket';//':44363/WebSocket';'/api/WebSocket'

const String args = '990d6953-774e-4ccd-98e0-73e7076d402c';


void main(List<String> args) =>
  socketHandler()
  .then(onDone, onError: onError);


Future<void> socketHandler() async {
  io.stdout.writeln('\n### НАЧАЛО');  
  io.stdout.writeln('Идет подключение к вебсокету \'$wsLiteral://$wsaddress$wspath\' ...');
  final io.WebSocket _webSocket = await io.WebSocket.connect('$wsLiteral://$wsaddress$wspath/$args');
  final Stream<dynamic> _webSocketStream = _webSocket.asBroadcastStream();

  if (_webSocket.readyState != 1) {
    io.stderr.writeln('Подключение не установлено!');
    throw UnsupportedError('Подключение с сервером не было установлено.');
  }
  
  _webSocketStream.last.whenComplete(() {
    print('Подключение сброшено.');
  });

  _webSocketStream.forEach((v) {
    io.stdout.writeln('> ${v.toString()}');
  });

  io.stdout.writeln('Подключение установлено.\nВведите сообщение или \'/q\' для выхода.');
  //String input = io.stdin.readLineSync();
  await io.stdin
    .transform<String>(utf8.decoder)
    .takeWhile((String input) {
      if (input.trim().toLowerCase() == '/q') {
        return false;
      }
      _webSocket.add(input);
      return true;
    })
    .drain()
    .whenComplete(() {
      _webSocket?.close(1000, 'That\'s enough c:');
    });
}


// Попрощаемся
Future<void> onDone([_]) async {
  io.stdout.writeln('\n### КОНЕЦ');
  await Future.delayed(const Duration(seconds: 5)).whenComplete(() {
    io.exit(io.exitCode);
  });
}


// Выведем непредвиденную ошибку
Future<void> onError(Object error) async {  
  io.stderr.writeln('\nНЕ ПРЕДВИДЕННАЯ ОШИБКА: ${error}');
  await Future.delayed(const Duration(seconds: 30)).whenComplete(() {
    io.exit(1);
  });
}
