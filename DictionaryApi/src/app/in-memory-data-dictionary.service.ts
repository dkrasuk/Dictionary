import { InMemoryDbService } from 'angular-in-memory-web-api';
export class InMemoryDataDictionaryService implements InMemoryDbService {
    constructor() {
        console.log('InMemoryDataDictionaryService ctor is init');
    }

  createDb() {
    const dictionaries = [
        /*{ Id: 54, Name: 'CollateralCarBodyType', Version: 1, Metadata: 'string', Items: [
            { Value: { Id: 3, Name: 'Хэтчбэк'}, ValueString: '{ "Id": "3", "Name": "Хэтчбэк"', ValueId: 3 },
            { Value: { Id: 6, Name: 'Пикап'}, ValueString: '{ "Id": "6", "Name": "Пикап"', ValueId: 6 },
            { Value: { Id: 1, Name: 'Седан'}, ValueString: '{ "Id": "1", "Name": "Седан"', ValueId: 1 }
        ]},
        { Id: 88, Name: 'CollateralCarBrand', Version: 1, Metadata: 'string', Items: [
            { Value: { Id: 123, Name: 'GMC'}, ValueString: '{ "Id": "123", "Name": "GMC"', ValueId: 123 },
            { Value: { Id: 28, Name: 'Honda'}, ValueString: '{ "Id": "28", "Name": "Honda"', ValueId: 28 },
            { Value: { Id: 32, Name: 'Jeep'}, ValueString: '{ "Id": "32", "Name": "Jeep"', ValueId: 32 }
        ]}*/
        { Id: 54, Name: 'CollateralCarBodyType', Version: '1', Metadata: 'string' },
        { Id: 88, Name: 'CollateralCarBrand', Version: '1', Metadata: 'string' }
    ];
    return {dictionaries};
  }
}
