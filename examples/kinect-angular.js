(function () {

    angular
        .module('myApp')
        .factory('kinect', ['$rootScope', myKinectFactory]);

    myKinectFactory.$inject = ['$rootScope'];

    function myKinectFactory($rootScope) {

        'use strict';

        var connection;

        function start() {

            if (typeof connection === 'undefined') {
                connection = new WebSocket('ws://localhost:3333/kinect');

                connection.onopen = function () {
                    console.log('Websocket open');
                    ping();
                };

                connection.onerror = function (error) {
                    console.log('WebSocket Error ' + error);
                };

                connection.onclose = function () {
                    console.log('Websocket closed');
                    connection = new WebSocket('ws://localhost:3333/kinect');
                };

                connection.onmessage = function (e) {
                    var bodies = JSON.parse(e.data);
                    if (bodies && bodies.length > 0) {
                        $rootScope.$emit('kinectBodies', bodies);
                    }
                };

            }

        }

        function ping() {
            connection.send('Ping');
            setTimeout(ping, 10000);
        }

        function handsUp(joints) {

            if (joints.HandLeft.Position.Y < joints.Head.Position.Y) {
                return false;
            }
            if (joints.HandRight.Position.Y < joints.Head.Position.Y) {
                return false;
            }
            return true;

        }

        function handsTogether(joints) {

            if (Math.abs(joints.HandLeft.Position.Y - joints.HandRight.Position.Y) < 0.1 &&
                Math.abs(joints.HandLeft.Position.X - joints.HandRight.Position.X) < 0.1) {
                return true;
            }
            return false;

        }

        return {
            start: start,
            ping: ping,
            handsUp: handsUp,
            handsTogether: handsTogether
        };
    }
})();