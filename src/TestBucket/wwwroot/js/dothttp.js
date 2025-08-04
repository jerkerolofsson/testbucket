
hljs.registerLanguage('http', function() {
  return {
    case_insensitive: false,
    keywords: 'GET POST PATCH PUT DELETE LIST CALL',
    contains: [
      {
        className: 'string',
        begin: '"',
        end: '"'
      },
      hljs.COMMENT(
        '^#', // begin
        '$', // end
        {
          keywords: ["not contains", "contains", "==", "!=", ">", "<", ">=", "<="],
          contains: [
            {
                className: 'verify',
                begin: '@verify',
                end: '$',

                contains: [
                    {
                        className: 'operator',
                        begin: 'not exists',
                    },
                    {
                        className: 'operator',
                        begin: 'not contains',
                    },
                    {
                        className: 'operator',
                        begin: 'contains',
                    },
                    {
                        className: 'operator',
                        begin: '==',
                    },
                    {
                        className: 'operator',
                        begin: '!=',
                    },
                    {
                        className: 'operator',
                        begin: '\>=',
                    },
                    {
                        className: 'operator',
                        begin: '\<=',
                    },
                    {
                        className: 'operator',
                        begin: '\<',
                    },
                    {
                        className: 'operator',
                        begin: '\>',
                    },
                    {
                        className: 'operator',
                        begin: 'exists',
                    },

                    {
                        className: 'verifier',
                        begin: 'mcp',
                    },
                    {
                        className: 'verifier',
                        begin: 'http',
                    },
                    {
                        className: 'verifier',
                        begin: 'header',
                    },
                    {
                        className: 'verifier',
                        begin: 'json',
                    },
                    {
                        className: 'verifier',
                        begin: 'tool',
                    },

                ]
            }, 
            {
                className: 'name',
                begin: '@name'
            },
            {
                className: 'name',
                begin: '@stage'
            }
          ]
        }
      )

    ]
  }
});