using System;
using System.Collections.Generic;
using System.Text;

namespace rlglnet
{
    public class FPScontrol
    {
        public double maxFPS { get; set; } = 60.0;
        double maxPeriod()
        {
            return 1.0 / maxFPS;
        }
        double _renderTime = 0.0;
        double _lastTime = 0.0;
        double _currentTime = 0.0;
        double _deltaTime = 0.0;
        const int _nFramesCount = 20;
        double[] _fpsValues = new double[_nFramesCount];
        int _currentFrame = 0;

        public bool Process()
        {
            _currentTime = GLFW.Glfw.Time;
            _deltaTime = _currentTime - _lastTime;
            if (_deltaTime >= maxPeriod())
            {
                _fpsValues[_currentFrame] = 1.0 / _deltaTime;

                if (_currentFrame == (_nFramesCount - 1))
                {
                    double sum = 0.0;
                    for (int i = 0; i < _nFramesCount; i++)
                    {
                        sum += _fpsValues[i];
                    }
                    double fpsAvg = sum / (double)_nFramesCount;
                    _currentFrame = 0;
                }
                _lastTime = _currentTime;
                _currentFrame++;
                return true;
            }
            return false;
        }


    }
}
