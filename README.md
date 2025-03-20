Service for Data Management (Save System)

Roadmap:
- test in project
  
- cleans code(and maybe example)
  
- сделать реализации быть более абстрактными:
  - отказаться от жесткой привязки к типам данных
  - например JsonStorage - изменить на FileStorage, принимающий сериализатор
    - таким образом Тип самого хранилища - файловое, все остальное не важно.
    - так же это отделит привязку к конкретным сериализаторам.
     
- можно сделать DataService более универсальным:
  - сейчас обязательны реализации трех хранилищ - кэш, файловое, удаленное - но любое можно заменить пустышкой. 
  - добавление хранилищ списком
    - тогда нужно будет добавить приоритет/порядок для загрузки и сохранений в конкретные хранилища
    - Синхронизацию тоже придется изменять для поддержки списка хранилищ


Requirements:
- UniTask: link
- Newtonsoft json: link

Features:
- Flexibility:
  - anymore data types(any serializable) - but need inherit from DataEntity!
  - custom realisations for Storages (LocalStorage > JsonStorage(simple files) | SecureJsonStorage | SQLstorage(use BD and etc))
- Auto Synchronization(works with queues)
- Forced Save(force save all data in queues)
- Load priority - Cache > Local > Remote
- Auto cached data - load data from server > auto saved in cache and local. TODO может сделать настройки у типов данных? кешируемый/сохраняемый локально/удаленно и тд 
- Key Naming support:
  - func KeyFor<TypeData>(anyStringParams[]) -> return string key ~ typedata_param1_param2_etc
  - use in LoadAll<TypeData>(KeyFor<TypeData>()) -> return all data with prefix - "typedata".
  - use in Save for good standart naming
- Check results operations(with class Result)
  - IsSuccefull
  - ErrorMessage
  - get Data
- Ability for added:
  - check versions(there is base logic)
  - check time updates
  - data validations

Features in example:
- remote storage use empty realization
- there is a load all data with type - usages LoadAll<TypeData>(KeyFor<TypeData>())
- UI for show loaded data
- class DataMonitorUI shows all data in Storages
