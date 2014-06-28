{
  'targets': [{
    'target_name': 'strider-msbuild-logger',
    'type': 'none',
    'sources': [ 'logger.cs' ],
    'actions': [{
      'action_name': 'compile',
      'inputs': [ 'logger.cs' ],
      'outputs': [ 'strider.msbuild.logger.dll' ],
      'message': 'msbuild Strider.MsBuild.Logger.csproj',
      'conditions': [
        ['OS=="win"', {
          'action': ['msbuild', 'Strider.MsBuild.Logger.csproj', '/nologo', '/tv:2.0', '/p:Configuration=Release']
        }]
      ]
    }]
  }]
}
