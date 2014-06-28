{
  'targets': [{
    'target_name': 'strider-msbuild-logger',
    'type': 'none',
    'actions': [{
      'action_name': 'compile',
      'inputs': [ 'logger.cs' ],
      'outputs': [ 'strider.msbuild.logger.dll' ],
      'message': 'msbuild Strider.MsBuild.Logger.csproj',
      'action': ['msbuild', 'Strider.MsBuild.Logger.csproj', '/nologo', '/tv:2.0', '/p:Configuration=Release']
    }]
  }]
}
