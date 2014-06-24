{
  'targets': [
    {
      'target_name': 'strider-msbuild-logger',
      'type': 'none',
      'sources': [ 'logger.cs' ],
      'actions': [
	    {
		  'action_name': 'compile',
		  'inputs': [ 'logger.cs' ],
		  'outputs': [ 'strider.msbuild.logger.dll' ],
		  'message': 'csc logger.cs -out:strider.msbuild.logger.dll',
		  'conditions': [
            ['OS=="win"', {
              'action': ['csc', '-target:library', '-out:..\\strider.msbuild.logger.dll', '-r:Microsoft.Build.Framework.dll', 'logger.cs']
              }, {
              'action': ['dmcs', '-sdk:2.0', '-target:library', '-out:..\\strider.msbuild.logger.dll', '-r:Microsoft.Build.Framework.dll', 'logger.cs']
              }
            ]
          ]
		}
	  ]
    }
  ]
}
